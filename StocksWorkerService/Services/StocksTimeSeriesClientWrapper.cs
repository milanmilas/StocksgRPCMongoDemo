using Grpc.Net.Client;
using StocksGrpcService;
using StocksWorkerService.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using grpc = global::Grpc.Core;

namespace StocksWorkerService.Services
{
    public class StocksTimeSeriesClientWrapper : IStocksTimeSeriesClientWrapper
    {
        private StocksTimeSeriesClientConfiguration _config;
        private StocksTimeSeries.StocksTimeSeriesClient _client;

        public StocksTimeSeriesClientWrapper(StocksTimeSeriesClientConfiguration config)
        {
            _config = config;
            var channel = GrpcChannel.ForAddress(config.ServerUrl);
            _client = new StocksTimeSeries.StocksTimeSeriesClient(channel);
        }

        public virtual grpc::AsyncUnaryCall<global::StocksGrpcService.StocksTimeSeriesCreateReply> CreateAsync(global::StocksGrpcService.StocksTimeSeriesCreateRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
        {
            return _client.CreateAsync(request, headers, deadline, cancellationToken);
        }
    }
}
