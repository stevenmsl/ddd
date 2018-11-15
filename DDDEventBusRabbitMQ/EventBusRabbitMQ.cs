using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using DDDEventBus;
using DDDEventBus.Abstractions;
using DDDEventBus.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Autofac;
using Microsoft.CSharp;


namespace DDDEventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BROKER_NAME = "ddd_event_bus";
        const string AUTOFACT_SCOPE_NAME = "ddd_event_bus";
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly ILifetimeScope _autofac;

        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            ILifetimeScope autofac,
            IEventBusSubscriptionsManager subsManager, string queueName = null)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _queueName = queueName;
            _autofac = autofac;
            _consumerChannel = createConsumerChannel();
            _subsManager.OnEventRemoved += subsManager_OnEventRemoved;

        }

        private void subsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.TryConnect();

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: BROKER_NAME,
                    routingKey: eventName);

                if (_subsManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    _consumerChannel.Close();
                }

            }

           
        }

        private IModel createConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            var channel = _persistentConnection.CreateModel();
            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

            channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await processEvent(eventName, message);

                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: _queueName,
                autoAck: false,
                consumer: consumer
                );


            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = createConsumerChannel();
            };

            return channel;
        }


        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
            _subsManager.Clear();
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                var eventName = @event.GetType().Name;
                channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; //persistent

                channel.BasicPublish(exchange: BROKER_NAME,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                    );
            }

        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            doInternalSubscription(eventName);
            _subsManager.AddSubscription<T, TH>();
        }

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            doInternalSubscription(eventName);
            _subsManager.AddDynamicSubscription<TH>(eventName);
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _subsManager.RemoveSubscription<T, TH>();
        }

        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(eventName);
        }

        private void doInternalSubscription(string eventName)
        {
            if (!_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var channel = _persistentConnection.CreateModel())
                {
                    channel.QueueBind(
                        queue: _queueName,
                        exchange: BROKER_NAME,
                        routingKey: eventName
                    );
                }
            }
        }

        private async Task processEvent(string eventName, string message)
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                //Autofac recommends resolve a service from a lifetime scope instead of from the root container to avoid potential memory leak.
                using (var scope = _autofac.BeginLifetimeScope(AUTOFACT_SCOPE_NAME))
                {
                    var subs = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var sub in subs)
                    {
                        if (sub.IsDynamic)
                        {
                            //Also a service may or may not registered, use conditional resolution methods such as ResolveOptional to resolve a service.    
                            //It is dynamic, so you don’t really have a event type per se. You can only cast the handler to IDynamicIntegrationEventHandler  
                            var handler = scope.ResolveOptional(sub.HandlerType) as IDynamicIntegrationEventHandler;
                            dynamic eventData = JObject.Parse(message);
                            await handler.Handle(eventData);
                        }
                        else
                        {
                            //The local variable eventType here is an instance of Type. You can’t use it in type casting directly; 
                            //you need to use reflection. 
                            var eventType = _subsManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var handler = scope.ResolveOptional(sub.HandlerType);
                            //Create an instance of a generic Type – the type of the event handler
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                        }
                    }
                }
            
            }
        }
    }
}
