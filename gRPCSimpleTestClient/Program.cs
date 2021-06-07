using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using StocksGrpcService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace gRPCSimpleTestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var _client = new StocksTimeSeries.StocksTimeSeriesClient(channel);

            var clientRequest = new StocksTimeSeriesGetRequest { Symbol = "AAPL" };
            clientRequest.Datasources.Add("iex");
            clientRequest.DateTimeFrom = Timestamp.FromDateTime(DateTime.Parse("07/06/2021 12:33:02Z").ToUniversalTime());


            using (var call = _client.Get(clientRequest))
            {
                while (await call.ResponseStream.MoveNext(CancellationToken.None))
                {
                    StocksTimeSeriesRecord current = call.ResponseStream.Current;
                    Console.WriteLine(current.Data);
                }
            }

            Console.ReadLine();
        }
    }
}
