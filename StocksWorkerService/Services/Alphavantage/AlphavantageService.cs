using Microsoft.Extensions.Logging;
using StocksWorkerService.Configurations;
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
        private readonly AlphavantageServiceConfiguration config;
        private readonly IHttpClientFactory httpClientFactory;

        public AlphavantageService(ILogger<AlphavantageService> logger, AlphavantageServiceConfiguration config, IHttpClientFactory httpClientFactory)
        {
            if(!config.Symbols.Any()) throw new ArgumentNullException("config.Symbols", "At least one symbol must be specified.");
            this.logger = logger;
            this.config = config;
            this.httpClientFactory = httpClientFactory;
        }

        public void Process()
        {
            var urls = config.Symbols.Select(symbol => String.Format(config.Url, symbol, config.Interval, config.ApiKey)).ToList();

            var res = GetStocks(urls).Result;
        }

        private async Task<ConcurrentBag<string>> GetStocks(List<string> urls)
        {
            var result = new ConcurrentBag<string>();
            // Polly retry policy set up in Program.cs
            var client = httpClientFactory.CreateClient("retryclient");
            // throttle set per service, could be injected if multipl services need to share it
            var throttler = new SemaphoreSlim(initialCount: config.ParallelRequests);

            IEnumerable<Task> tasks = urls.Select(async url =>
            {
                await throttler.WaitAsync();
                try
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        result.Add(content);

                        logger.LogError("some error");
                        logger.LogWarning("some warn");
                    }
                    else
                    {

                    }
                }
                finally
                {
                    throttler.Release();
                }
            });
            await Task.WhenAll(tasks);

            return result;
        }
    }
}
