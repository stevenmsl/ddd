using System;
using System.Collections.Generic;
using System.Linq;
using DDDEventBus.Abstractions;
using DDDEventBus.Events;

namespace DDDEventBus
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;

        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }


        public bool IsEmpty => !_handlers.Keys.Any();

        public event EventHandler<string> OnEventRemoved;

        public void AddDynamicSubscription<TH>(string evenName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public void Clear() => _handlers.Clear();

        public string GetEventKey<T>() => typeof(T).Name;

        public Type GetEventTypeByName(string eventName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
        {
            throw new NotImplementedException();
        }

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            throw new NotImplementedException();
        }

        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);
      
        public void RemoveDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }




    }
}
