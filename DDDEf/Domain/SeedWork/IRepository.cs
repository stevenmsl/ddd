using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDDEf.Domain.SeedWork
{
    public class IRepository<T> where T : IAggregateRoot 
    {
        IUnitOfWork UnitOfWork { get; }
    }

}
