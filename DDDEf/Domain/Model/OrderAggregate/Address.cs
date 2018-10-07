using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DDDEf.Domain.SeedWork;

namespace DDDEf.Domain.Model.OrderAggregate
{
    public class Address : ValueObject
    {

        //Read-only properties are not mapped by convention. 
        //So, if you declare Street property like the following:
        //public String Street { get; }
        //No column will be created to map the Street property in the table created to map the entity type 
        //that owns an Address type value object.
        public String Street { get; private set; }
        public String City { get; private set; }
        public String State { get; private set; }
        public String Country { get; private set; }
        public String ZipCode { get; private set; }

        private Address() { }

        public Address(string street, string city, string state, string country, string zipcode)
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            // Using yield to define an iterator removes the need for an explicit extra class 
            //when you implement the IEnumerable and IEnumerator pattern for a custom collection type.
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
        }
    }
}
