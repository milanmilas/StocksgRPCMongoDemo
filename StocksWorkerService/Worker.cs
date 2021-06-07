using Grpc.Net.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StocksGrpcService;
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
                var input = new HelloRequest { Name = "Tim" };
                var channel = GrpcChannel.ForAddress("https://localhost:5001");
                var client = new Greeter.GreeterClient(channel);

                var reply = await client.SayHelloAsync(input);
                _logger.LogInformation(reply.Message);

                var client2 = new StocksTimeSeries.StocksTimeSeriesClient(channel);

                var request2 = new StocksTimeSeriesCreateRequest();
                request2.StocksTimeSeries.Add(new StocksTimeSeriesRecord { Symbol = "MM", DateTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow) });

                var reply2 = await client2.CreateAsync(request2);
                var reply3 = await client2.CreateAsync(request2);
                _logger.LogInformation(reply2.Status.ToString());

                //foreach (var service in services)
                //{
                //    await service.GetStocks(stoppingToken);
                //}

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(100000, stoppingToken);
            }
        }

        public void Run()
        {

        }
    }
}
