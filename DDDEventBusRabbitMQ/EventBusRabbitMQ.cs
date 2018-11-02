using System;
using System.Collections.Generic;
using System.Text;
using DDDEventBus;
using DDDEventBus.Abstractions;
using DDDEventBus.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Microsoft.Extensions.Logging;


namespace DDDEventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BROKER_NAME = "ddd_event_bus";
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            IEventBusSubscriptionsManager subsManager, string queueName = null)
        {
            _persistentConnection = persistentConnection;
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _queueName = queueName;
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
            throw new NotImplementedException();
        }

        public void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }
    }
}
