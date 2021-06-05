using StocksWorkerService.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StocksWorkerService.Services.Alphavantage
{
    public class AlphavantageService : IAlphavantageService
    {
        private readonly AlphavantageServiceConfiguration config;
        private readonly IHttpClientFactory httpClientFactory;

        public AlphavantageService(AlphavantageServiceConfiguration config, IHttpClientFactory httpClientFactory)
        {
            this.config = config;
            this.httpClientFactory = httpClientFactory;
        }

        public void Process()
        {

        }
    }
}
