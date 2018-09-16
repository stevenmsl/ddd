using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace DDDMediatR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly IMediator _mediator;

        public ValuesController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }
        
        [HttpGet("publish/{message}")]
        public async Task<ActionResult<string>> Publish(string message)
        {
            await _mediator.Publish<SomeEvent>(new SomeEvent(message));
            return "event published";
        }

        [HttpGet("response/")]
        public async Task<ActionResult<string>> Send()
        {
            return await _mediator.Send(new SomeRequest());
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
