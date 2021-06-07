using Microsoft.Extensions.Logging;
using StocksWorkerService.Configurations;
using StocksWorkerService.Model;
using StocksWorkerService.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StocksWorkerService.Services.Alphavantage
{
    public class StocksService<T> : IStocksService<T>
    {
        private readonly ILogger<StocksService<T>> logger;
        private readonly WebApiConfiguration config;
        private readonly IUrlBuilder<T> urlBuilder;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IDateTime dateTime;

        public StocksService(ILogger<StocksService<T>> logger,
                                WebApiConfiguration config, 
                                IUrlBuilder<T> urlBuilder,
                                IHttpClientFactory httpClientFactory,
                                IDateTime dateTime)
        {
            this.logger = logger;
            this.config = config;
            this.urlBuilder = urlBuilder;
            this.httpClientFactory = httpClientFactory;
            this.dateTime = dateTime;
        }

        public async Task<ConcurrentBag<Stock>> GetStocks(CancellationToken stoppingToken = default)
        {
            return await GetStocks(urlBuilder.BuildSymbolsUrls(), stoppingToken);
        }

        private async Task<ConcurrentBag<Stock>> GetStocks(List<(string symbol, string url)> sybolsUrls, CancellationToken stoppingToken)
        {
            logger.LogDebug($"Start getting Stocks.");

            var result = new ConcurrentBag<Stock>();
            // Polly retry policy set up in Program.cs
            var client = httpClientFactory.CreateClient("retryclient");
            // throttle set per service, could be injected if multipl services need to share it
            var throttler = new SemaphoreSlim(initialCount: config.ParallelRequests);

            IEnumerable<Task> tasks = sybolsUrls.Select(async symbolUrl =>
            {
                try
                {
                    if (stoppingToken.IsCancellationRequested) return;

                    await throttler.WaitAsync(stoppingToken);

                    var response = await client.GetAsync(symbolUrl.url, stoppingToken);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync(stoppingToken);
                        result.Add(new Stock {
                            DataSource = urlBuilder.DataSource,
                            Symbol = symbolUrl.symbol,
                            DateTime = dateTime.UtcNow(),
                            Data = content
                        });
                    }
                    else
                    {
                        logger.LogError($"An error occured while requesting symbol '{symbolUrl.symbol}' with url '{symbolUrl.url}' with response status code '{response.StatusCode}'");
                    }
                }
                catch(OperationCanceledException oe)
                {
                    logger.LogWarning("Operation has been cancelled by stoppingToken.");
                }
                catch(Exception e)
                {
                    logger.LogError(e, $"An exception has occured while getting stocks ulr: '{symbolUrl.url}' | message'{e.Message}'.");
                }
                finally
                {
                    throttler.Release();
                }
            });

            await Task.WhenAny(Task.WhenAll(tasks), stoppingToken.AsTask());
            logger.LogDebug($"End getting Stocks.");
            return result;
        }
    }
}
