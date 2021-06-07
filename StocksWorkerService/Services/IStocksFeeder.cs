using System.Threading;
using System.Threading.Tasks;

namespace StocksWorkerService.Services
{
    public interface IStocksFeeder<T>
    {
        Task Proccess(CancellationToken stoppingToken = default);
    }
}
