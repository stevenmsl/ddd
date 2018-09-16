using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DDDMediatR
{
    public class RequestHandlerB : IRequestHandler<SomeRequest, string>
    {
        #pragma warning disable 1998
        public async Task<string> Handle(SomeRequest request, CancellationToken cancellationToken)
        {
            return "Handled by RequestHandlerB";
        }
        #pragma warning restore 1998
    }
}
