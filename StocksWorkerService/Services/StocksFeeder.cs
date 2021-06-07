using Microsoft.Extensions.Logging;
using StocksGrpcService;
using StocksWorkerService.Model;
using StocksWorkerService.Services.Alphavantage;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace StocksWorkerService.Services
{
    public class StocksFeeder<T>: IStocksFeeder<T>
    {
        private readonly ILogger<StocksFeeder<T>> _logger;
        private readonly IStocksService<T> _stocksService;
        private readonly IStocksTimeSeriesClientWrapper _grpcClient;
        private readonly IConverter<Stock, StocksTimeSeriesRecord> _converter;

        public StocksFeeder(ILogger<StocksFeeder<T>> logger,
                            IStocksService<T> stocksService,
                            IStocksTimeSeriesClientWrapper grpcClient,
                            IConverter<Stock, StocksTimeSeriesRecord> converter)
        {
            _logger = logger;
            _stocksService = stocksService;
            _grpcClient = grpcClient;
            _converter = converter;
        }

        public async Task Proccess(CancellationToken stoppingToken = default)
        {
            try
            {
                _logger.LogInformation("Start Processing");
                ConcurrentBag<Stock> stocks = await _stocksService.GetStocks(stoppingToken);
                if (stocks == null || stocks.Count == 0)
                {
                    _logger.LogWarning("No Stocks data returned from stocks service.");
                    return;
                }

                var records = _converter.Convert(stocks.ToArray());

                var request = new StocksTimeSeriesCreateRequest();
                request
                    .StocksTimeSeries
                    .Add(records);

                await _grpcClient.CreateAsync(request, cancellationToken: stoppingToken);
                _logger.LogInformation("End Processing");
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, $"An Error occured while running Stocks Feeder '{e.Message}'.");
            }
        }
    }
}
