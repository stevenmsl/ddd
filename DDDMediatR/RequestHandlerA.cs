using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DDDMediatR
{
    //When you send a request, one and only one handler will be called and will return a response of the appropriate type. 
    public class RequestHandlerA : IRequestHandler<SomeRequest, string>
    {
        #pragma warning disable 1998
        public async Task<string> Handle(SomeRequest request, CancellationToken cancellationToken)
        {
            return "Handled by RequestHandlerA";
        }
        #pragma warning restore 1998
    }
}
