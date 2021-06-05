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
        private readonly IAlphavantageService service;

        public Worker(ILogger<Worker> logger, IAlphavantageService service)
        {
            _logger = logger;
            this.service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
