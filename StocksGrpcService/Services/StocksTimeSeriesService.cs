using Grpc.Core;
using Microsoft.Extensions.Logging;
using StocksGrpcService.DataAccess;
using StocksGrpcService.DataAccess.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StocksGrpcService
{

    public class StocksTimeSeriesService : StocksTimeSeries.StocksTimeSeriesBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly IMongoRepository<StockTimeSeries> _repository;
        private readonly IConverter<StocksTimeSeriesRecord, StockTimeSeries> _converter;

        public StocksTimeSeriesService(ILogger<GreeterService> logger,
            IMongoRepository<StockTimeSeries> repository,
            IConverter<StocksTimeSeriesRecord, StockTimeSeries> converter)
        {
            _logger = logger;
            _repository = repository;
            _converter = converter;
        }

        public override async Task<StocksTimeSeriesCreateReply> Create(StocksTimeSeriesCreateRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received Create Stocks Time Series.");
            try
            {

                var stockTimeSeries = _converter.Convert(request.StocksTimeSeries);
                await _repository.InsertManyAsync(stockTimeSeries);

                return new StocksTimeSeriesCreateReply
                {
                    Status = StatusCode.Ok,
                    Message = "created"
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation("Received Create Stocks Time Series.");

                return new StocksTimeSeriesCreateReply
                {
                    Status = StatusCode.Error,
                    Message = e.Message
                };
            }
        }
    }
}
