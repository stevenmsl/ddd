using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDDAutofac
{
    public class TransientQueries : IQueries
    {
        private Guid guid;

        public TransientQueries()
        {
            guid = Guid.NewGuid();
        }

        public string GetGuid()
        {           
            return guid.ToString();                
        }
    }
}
