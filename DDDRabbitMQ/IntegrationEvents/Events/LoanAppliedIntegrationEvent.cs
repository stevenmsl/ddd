using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDEventBus.Events;

namespace DDDRabbitMQ.IntegrationEvents.Events
{
    public class LoanAppliedIntegrationEvent : IntegrationEvent
    {
        public string LoanId { get; set;}

        public LoanAppliedIntegrationEvent(string loanId) => LoanId = loanId;

    }
}
