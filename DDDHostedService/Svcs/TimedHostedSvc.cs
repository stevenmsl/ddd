using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace DDDHostedService.Svcs
{
    internal class TimedHostedSvc : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        public TimedHostedSvc(ILogger<TimedHostedSvc> logger)
        {
            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting timed background service…{Environment.NewLine}");
            _timer = new Timer(doWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private void doWork(object state)
        {
            _logger.LogInformation($"Timed background service at work - {DateTime.Now}{Environment.NewLine}");

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stopping timed background service…{Environment.NewLine}");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
            
        }
    }
}
