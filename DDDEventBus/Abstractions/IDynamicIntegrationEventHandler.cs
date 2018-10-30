using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDDEventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
