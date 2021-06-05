using System.Collections.Generic;

namespace StocksWorkerService.Services
{
    public interface IUrlBuilder
    {
        public string DataSource { get; }
        List<(string symbol, string url)> BuildSymbolsUrls();
    }
}