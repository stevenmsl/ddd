using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DDDEf.Domain.SeedWork;

namespace DDDEf.Domain.Model.OrderAggregate
{
    public class Address : ValueObject
    {

        public String Street { get; }
        public String City { get; }
        public String State { get; }
        public String Country { get; }
        public String ZipCode { get; }

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
