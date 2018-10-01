using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDDEf.Domain.SeedWork
{
    public abstract class ValueObject
    {
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            //If and only if one of the operands (left or right) is null
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false; //then we return false
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }
        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !(EqualOperator(left, right));
        }
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }
            ValueObject other = (ValueObject)obj;
            IEnumerator<object> thisValues = GetAtomicValues().GetEnumerator();
            IEnumerator<object> otherValues = other.GetAtomicValues().GetEnumerator();
            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (ReferenceEquals(thisValues.Current, null) ^ ReferenceEquals(otherValues.Current, null))
                {
                    return false;
                }
                if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
                {
                    return false;
                }
            }
            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        protected abstract IEnumerable<object> GetAtomicValues();

        public override int GetHashCode()
        {
            //Need to find out why this is implemented this way
            return GetAtomicValues()
             .Select(x => x != null ? x.GetHashCode() : 0)
             .Aggregate((x, y) => x ^ y);
        }
        public ValueObject GetCopy()
        {
            return this.MemberwiseClone() as ValueObject;
        }

    }
}
