using Google.Protobuf.WellKnownTypes;
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

        public override async Task Get(StocksTimeSeriesGetRequest request, IServerStreamWriter<StocksTimeSeriesRecord> responseStream, ServerCallContext context)
        {
            try
            {
                // can be refactored with Query and QueryHandler
                var dateTimeFrom = request.DateTimeFrom?.ToDateTime();
                var dateTimeTo = request.DateTimeTo?.ToDateTime();

                var result = _repository.Get((x) =>
                    x.Symbol == request.Symbol &&
                    (!request.Datasources.Any() || request.Datasources.Contains(x.DataSource)) &&
                    (request.DateTimeFrom == null || dateTimeFrom <= x.DateTime) &&
                    (request.DateTimeTo == null || x.DateTime <= dateTimeTo)
                    );

                foreach (var item in result)
                {
                    // additional Converter needed
                    var record = new StocksTimeSeriesRecord();

                    record.Datasource = item.DataSource;
                    record.Symbol = item.Symbol;
                    record.DateTime = Timestamp.FromDateTime(item.DateTime);
                    record.Data = item.Data;

                    await responseStream.WriteAsync(record);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while getting StockTimeSeries data.");
                throw;
            }
            
        }
    }
}
