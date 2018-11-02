﻿using System;
using RabbitMQ.Client;


namespace DDDEventBusRabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();   
    }
}
