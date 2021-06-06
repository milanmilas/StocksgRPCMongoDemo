using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksGrpcService
{
    public class StocksTimeSeriesService : StocksTimeSeries.StocksTimeSeriesBase
    {
        private readonly ILogger<GreeterService> _logger;
        public StocksTimeSeriesService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<StocksTimeSeriesCreateReply> Create(StocksTimeSeriesCreateRequest request, ServerCallContext context)
        {
            return Task.FromResult(new StocksTimeSeriesCreateReply
            {
                Status = StatusCode.Ok,
                Message = "created"
            });
        }
    }
}
