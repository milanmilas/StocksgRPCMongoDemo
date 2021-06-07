using Grpc.Net.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StocksGrpcService;
using StocksWorkerService.Schedulers;
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
        private readonly IEnumerable<IScheduler> _schedulers;

        public Worker(ILogger<Worker> logger, IEnumerable<Schedulers.IScheduler> schedulers)
        {
            _logger = logger;
            _schedulers = schedulers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var scheduler in _schedulers)
            {
                _logger.LogInformation($"Worker stopping scheduler '{scheduler.GetType().Name}'");
                scheduler.Start(stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(100000, stoppingToken);
            }

            foreach (var scheduler in _schedulers)
            {
                _logger.LogInformation($"Worker stopping scheduler {scheduler.GetType().Name}");
                scheduler.Stop();
            }

            _logger.LogInformation("Worker waiting 10 to stop service: {time}", DateTimeOffset.Now);
            await Task.Delay(10000, stoppingToken);
        }
    }
}
