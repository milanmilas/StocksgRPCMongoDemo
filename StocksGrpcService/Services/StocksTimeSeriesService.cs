using Grpc.Core;
using Microsoft.Extensions.Logging;
using StocksGrpcService.DataAccess;
using StocksGrpcService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksGrpcService
{
    public class StocksTimeSeriesService : StocksTimeSeries.StocksTimeSeriesBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly IMongoRepository<StockTimeSeries> _repository;

        public StocksTimeSeriesService(ILogger<GreeterService> logger, IMongoRepository<StockTimeSeries> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public override async Task<StocksTimeSeriesCreateReply> Create(StocksTimeSeriesCreateRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received Create Stocks Time Series.");
            try
            {
                var stocksTimeSeriesRecords = request.StocksTimeSeries.ToList();

                List<StockTimeSeries> stocksTimeSeries = new List<StockTimeSeries>();
                foreach (var record in stocksTimeSeriesRecords)
                {
                    var stockTimeSeries = new StockTimeSeries();
                    stockTimeSeries.DataSource = record.Datasource;
                    stockTimeSeries.Symbol = record.Symbol;
                    stockTimeSeries.DateTime = record.DateTime.ToDateTime();
                    stockTimeSeries.Data = record.Data;

                    stocksTimeSeries.Add(stockTimeSeries);
                }

                await _repository.InsertManyAsync(stocksTimeSeries);

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
