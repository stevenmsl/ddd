using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;


namespace DDDMediatR
{
    public class SomeEvent : INotification
    {
        public SomeEvent(string message)
        {
            Message = message;
        }
        public string Message { get; }
    } 

}
