using System;
using StocksGrpcService.DataAccess;

namespace StocksGrpcService.DataAccess.Models
{
    [BsonCollection("stockstimeseries")]
    public class StockTimeSeries : Document
    {
        public string DataSource { get; set; }
        public string Symbol { get; set; }
        public DateTime DateTime { get; set; }
        public string Data { get; set; }
    }
}
