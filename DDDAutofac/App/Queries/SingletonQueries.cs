using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDDAutofac
{
    public class SingletonQueries : IQueries
    {
        private Guid guid;

        public SingletonQueries()
        {
            guid = Guid.NewGuid();
        }

        public string GetGuid()
        {           
            return guid.ToString();                
        }
    }
}
