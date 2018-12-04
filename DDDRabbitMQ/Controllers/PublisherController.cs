using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using DDDEventBus.Abstractions;
using DDDIEPublisher.IntegrationEvents.Events;


namespace DDDIEPublisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private IEventBus _eventBus; 

        public PublisherController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [HttpGet("init")]
        public ActionResult<string> Init()
        {
            return "done";
        }

        [HttpGet("publish/{command}")]
        public ActionResult<string> Publish(string command)
        {
            var eventMessage = new LoanAppliedIntegrationEvent(Guid.NewGuid().ToString());


            _eventBus.Publish(eventMessage);

            return "published";
        }

    }
}