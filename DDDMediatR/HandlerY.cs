using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DDDMediatR
{
    public class HandlerY : INotificationHandler<SomeEvent>
    {
        private readonly ILogger<HandlerY> _logger;

        public HandlerY(ILogger<HandlerY> logger)
        {
            _logger = logger;
        }
        #pragma warning disable 1998
        public async Task Handle(SomeEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning($"HandlerY Handled: {notification.Message}");
        }
        #pragma warning restore 1998
    }
}
