using System;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace DDDRabbitMQ.MQ
{
    public class DefaultConnection : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultConnection> _logger;
        IConnection _connection;
        bool _disposed;

        public DefaultConnection(IConnectionFactory connectionFactory,
            ILogger<DefaultConnection> logger)
        {
            

        }


        public bool IsConnected => throw new NotImplementedException();

        public IModel CreateModel()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool TryConnect()
        {
            throw new NotImplementedException();
        }
    }
}
