using StocksGrpcService;
using StocksWorkerService.Model;
using StocksWorkerService.Utils;
using System.Collections.Generic;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;

namespace StocksWorkerService.Converters
{
    public class StocksToStocksTimeSeriesRecordConverter : IConverter<Stock, StocksTimeSeriesRecord>
    {
        private readonly IDateTime _dateTime;

        public StocksToStocksTimeSeriesRecordConverter(IDateTime dateTime)
        {
            _dateTime = dateTime;
        }
        public List<StocksTimeSeriesRecord> Convert(IEnumerable<Stock> items)
        {
            var result = new List<StocksTimeSeriesRecord>();
            foreach (var item in items)
            {
                var record = new StocksTimeSeriesRecord();

                record.Datasource = item.DataSource;
                record.Symbol = item.Symbol;
                record.DateTime = Timestamp.FromDateTime(item.DateTime);
                record.Data = item.Data;
                record.Timestamp = Timestamp.FromDateTime(_dateTime.UtcNow());
            }

            return result;
        }
    }
}
