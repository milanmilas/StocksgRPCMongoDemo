using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StocksWorkerService.Services.Alphavantage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StocksWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEnumerable<IStocksService> services;

        public Worker(ILogger<Worker> logger, IEnumerable<IStocksService> services)
        {
            _logger = logger;
            this.services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var service in services)
                {
                    await service.Process(stoppingToken);
                }
                
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(100000, stoppingToken);
            }
        }
    }
}
