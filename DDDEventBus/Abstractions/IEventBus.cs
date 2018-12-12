using System;
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
        /// <summary>
        /// Register an event type so later events of the specified event type can be published.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Register<T>() where T : IntegrationEvent;
        void UnRegister<T>() where T : IntegrationEvent;

    }
}
