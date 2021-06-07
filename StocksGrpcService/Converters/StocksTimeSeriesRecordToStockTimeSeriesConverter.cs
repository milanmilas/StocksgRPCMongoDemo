using StocksGrpcService.DataAccess.Models;
using System.Collections.Generic;

namespace StocksGrpcService.Converters
{

    public class StocksTimeSeriesRecordToStockTimeSeriesConverter : IConverter<StocksTimeSeriesRecord, StockTimeSeries>
    {
        public List<StockTimeSeries> Convert(IEnumerable<StocksTimeSeriesRecord> stocksTimeSeriesRecords)
        {
            // add logging with data flush for item that fails to convert
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

            return stocksTimeSeries;
        }
    }
}
