using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDEventBus.Abstractions;
using DDDIESubscriber.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;

namespace DDDIESubscriber.IntegrationEvents.EventHandling
{
    public class LoanAppliedIntegrationEventHandler : IIntegrationEventHandler<LoanAppliedIntegrationEvent>
    {
        private readonly ILogger<LoanAppliedIntegrationEventHandler> _logger;

        public LoanAppliedIntegrationEventHandler(ILogger<LoanAppliedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(LoanAppliedIntegrationEvent @event)
        {
            await Task.Run(
                () => _logger.LogInformation($"LoanAppliedIntegrationEvent received: {@event.LoanId}")
            );
            
        }
    }
}
