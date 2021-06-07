using Grpc.Core;
using StocksGrpcService;
using System;
using System.Threading;

namespace StocksWorkerService.Services
{
    public interface IStocksTimeSeriesClientWrapper
    {
        AsyncUnaryCall<StocksTimeSeriesCreateReply> CreateAsync(StocksTimeSeriesCreateRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
    }
}