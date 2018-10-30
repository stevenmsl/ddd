using System;
using System.Threading.Tasks;
using DDDEventBus.Events;

namespace DDDEventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }
    public interface IIntegrationEventHandler
    {
    }
}
