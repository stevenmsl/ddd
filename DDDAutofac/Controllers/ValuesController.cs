using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DDDAutofac.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        SingletonQueries _single;
        TransientQueries _transient;


        public ValuesController( SingletonQueries single, TransientQueries transient)
        {
            _single = single;
            _transient = transient;
        }

        /// <summary>
        /// Should return the same GUID every time
        /// </summary>
        /// <returns></returns>
        [HttpGet("singleton")]
        public string Singleton()
        {
            return _single.GetGuid();
        }

        /// <summary>
        /// Should return different GUID every time
        /// </summary>
        /// <returns></returns>
        [HttpGet("transient")]
        public string transient()
        {
            return  _transient.GetGuid();
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
