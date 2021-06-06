using StocksWorkerService.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWorkerService.Services.Alphavantage
{
    public class IexUrlBuilder : IUrlBuilder<IexServiceConfiguration>
    {
        private readonly IexServiceConfiguration config;

        public IexUrlBuilder(IexServiceConfiguration config)
        {
            if (!config.Symbols.Any()) throw new ArgumentNullException("config.Symbols", "At least one symbol must be specified.");

            this.config = config;
        }

        public string DataSource => config.DataSource;

        public List<(string symbol, string url)> BuildSymbolsUrls()
        {
            var urls = config.Symbols.Select(symbol => (symbol, String.Format(config.Url, symbol, config.Token))).ToList();
            return urls;
        }
    }
}
