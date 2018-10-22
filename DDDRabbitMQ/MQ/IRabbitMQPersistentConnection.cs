using System;
using RabbitMQ.Client;


namespace DDDRabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();   
    }
}
