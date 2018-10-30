﻿using System;
using System.Collections.Generic;
using System.Text;
using DDDEventBus.Events;

namespace DDDEventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);
        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
        void Unsubscribe<T,TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
        void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;
        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

    }
}
