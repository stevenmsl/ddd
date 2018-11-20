using System;
using System.IO;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace DDDEventBusRabbitMQ
{
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        IConnection _connection;
        bool _disposed;
        object sync_root = new object();

        public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory,
            ILogger<DefaultRabbitMQPersistentConnection> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            
        }


        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No connections available to perform CreateModel action");
            }
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try
            {
                _connection.Dispose();
            }
            catch (IOException ex) //Why only interested in catching IO exception?
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect...");
            lock (sync_root)
            {
               try
               {
                    _connection = _connectionFactory.CreateConnection();

                    if (IsConnected)
                    {
                        _connection.ConnectionShutdown += _connection_ConnectionShutdown;
                        _connection.CallbackException += _connection_CallbackException;
                        _connection.ConnectionBlocked += _connection_ConnectionBlocked;
                        _logger.LogInformation
                            ($"RabbitMQ persistent connection acquired a connection {_connection.Endpoint.HostName} and is subscribed to failure events");
                        return true;
                    }
                    else
                        return false;
               }
               catch (Exception ex)
               {
                    _logger.LogCritical
                        ($"FATAL ERROR: RabbitMQ connections could not be created and opened. {ex.ToString()}");
                    return false;
               }

            }
           
        }

        private void _connection_ConnectionBlocked(object sender, RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
        }

        private void _connection_CallbackException(object sender, RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        private void _connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }
    }
}
