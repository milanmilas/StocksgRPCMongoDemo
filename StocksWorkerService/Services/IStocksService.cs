using StocksWorkerService.Model;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace StocksWorkerService.Services.Alphavantage
{
    public interface IStocksService
    {
        Task<ConcurrentBag<Stock>> GetStocks(CancellationToken stoppingToken);
    }
}