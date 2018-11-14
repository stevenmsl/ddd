﻿using System;
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
            _persistentConnection = persistentConnection;
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
                using (var scope = _autofac.BeginLifetimeScope(AUTOFACT_SCOPE_NAME))
                {
                    var subs = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var sub in subs)
                    {
                        if (sub.IsDynamic)
                        {
                            var handler = scope.ResolveOptional(sub.HandlerType) as IDynamicIntegrationEventHandler;
                            dynamic eventData = JObject.Parse(message);
                            await handler.Handle(eventData);
                        }
                        else
                        {
                            var eventType = _subsManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var handler = scope.ResolveOptional(sub.HandlerType);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                        }
                    }
                }
            
            }
        }
    }
}
