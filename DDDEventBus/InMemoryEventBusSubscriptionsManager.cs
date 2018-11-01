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

        private void raiseOnEventRemoved(string eventName)
        {
            if (OnEventRemoved != null)
                OnEventRemoved(this, eventName);
        }


        public void AddDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            doAddSubscription(typeof(TH), eventName, true);
        }

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            doAddSubscription(typeof(TH), GetEventKey<T>(), false);
            _eventTypes.Add(typeof(T));

        }

        private void doAddSubscription(Type handlerType, string eventName, bool isDynamic)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }
            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
                throw new ArgumentException
                    ($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));

            _handlers[eventName].Add(isDynamic ? SubscriptionInfo.Dynamic(handlerType) : SubscriptionInfo.Typed(handlerType));
           
        }

        public void Clear() => _handlers.Clear();

        public string GetEventKey<T>() => typeof(T).Name;

        public Type GetEventTypeByName(string eventName) => 
            _eventTypes.SingleOrDefault(t => t.Name == eventName);
       

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            return GetHandlersForEvent(GetEventKey<T>());
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];
        
        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
            => HasSubscriptionsForEvent(GetEventKey<T>());



        public bool HasSubscriptionsForEvent(string eventName) =>
            //When the very last subscription is removed, the event entry itself will be removed from the dictionary
            _handlers.ContainsKey(eventName); 

        public void RemoveDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            doRemoveHandler(eventName, findDynamicSubsToRemove<TH>(eventName));
        }

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            doRemoveHandler(GetEventKey<T>(), findSubsToRemove<T, TH>());
        }

        #region private methods to support removal of subscriptions
        private void doRemoveHandler(string eventName, SubscriptionInfo subsToRemove)
        {
            if (subsToRemove != null)
            {
                _handlers[eventName].Remove(subsToRemove);
                //If there is no longer has any handler subscribed to this event, remove this event
                if (!_handlers[eventName].Any())
                {
                    _handlers.Remove(eventName);
                    var evenType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                    if (evenType != null)
                        _eventTypes.Remove(evenType);
                    raiseOnEventRemoved(eventName);
                }
            }
        }

        private SubscriptionInfo findDynamicSubsToRemove<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            return doFindSubsToRemove(eventName, typeof(TH));
        }
        private SubscriptionInfo findSubsToRemove<T, TH>() 
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            return doFindSubsToRemove(GetEventKey<T>(), typeof(TH));
        }
        private SubscriptionInfo doFindSubsToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionsForEvent(eventName)) return null;
            return _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        #endregion




    }
}
