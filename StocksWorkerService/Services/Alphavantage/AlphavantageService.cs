using Microsoft.Extensions.Logging;
using StocksWorkerService.Configurations;
using StocksWorkerService.Model;
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
    public class AlphavantageService : IAlphavantageService
    {
        private readonly ILogger<AlphavantageService> logger;
        private readonly WebApiConfiguration config;
        private readonly IUrlBuilder urlBuilder;
        private readonly IHttpClientFactory httpClientFactory;

        public AlphavantageService(ILogger<AlphavantageService> logger,
                                   WebApiConfiguration config, 
                                   IUrlBuilder urlBuilder, 
                                   IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.config = config;
            this.urlBuilder = urlBuilder;
            this.httpClientFactory = httpClientFactory;
        }

        public void Process()
        {
            var res = GetStocks(urlBuilder.BuildSymbolsUrls()).Result;
        }

        private async Task<ConcurrentBag<Stock>> GetStocks(List<(string symbol, string url)> sybolsUrls)
        {
            logger.LogDebug($"Start getting Stocks.");

            var result = new ConcurrentBag<Stock>();
            // Polly retry policy set up in Program.cs
            var client = httpClientFactory.CreateClient("retryclient");
            // throttle set per service, could be injected if multipl services need to share it
            var throttler = new SemaphoreSlim(initialCount: config.ParallelRequests);

            IEnumerable<Task> tasks = sybolsUrls.Select(async symbolUrl =>
            {
                await throttler.WaitAsync();
                try
                {
                    var response = await client.GetAsync(symbolUrl.url);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        result.Add(new Stock {
                            DataSource = urlBuilder.DataSource,
                            Symbol = symbolUrl.symbol,
                            DateTime = DateTime.UtcNow,
                            Data = content
                        });
                    }
                    else
                    {
                        logger.LogError($"An error occured while requesting symbol '{symbolUrl.symbol}' with url '{symbolUrl.url}' with response status code '{response.StatusCode}'");
                    }
                }catch(Exception e)
                {
                    logger.LogError($"An exception has occured while getting stocks ulr: '{symbolUrl.url}' | message'{e.Message}'.", e);
                }
                finally
                {
                    throttler.Release();
                }
            });
            await Task.WhenAll(tasks);

            logger.LogDebug($"End getting Stocks.");
            return result;
        }
    }
}
