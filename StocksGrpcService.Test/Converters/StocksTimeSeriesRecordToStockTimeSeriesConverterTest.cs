using MongoDB.Driver;
using NUnit.Framework;
using Shouldly;
using StocksGrpcService.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;

namespace StocksGrpcService.Test.Converters
{
    [TestFixture]
    public class StocksTimeSeriesRecordToStockTimeSeriesConverterTest
    {
        private StocksTimeSeriesRecordToStockTimeSeriesConverter _cut;

        [SetUp]
        public void Setup()
        {
            _cut = new StocksTimeSeriesRecordToStockTimeSeriesConverter();
        }

        [Test]
        public void Converting_empty_collection_of_StocksTimeSeriesRecords_Should_return_an_emtpy_collection_of_StockTimeSeries()
        {
            List<StocksTimeSeriesRecord> records = new List<StocksTimeSeriesRecord>();
            var result = _cut.Convert(records);

            result.ShouldBeEmpty();
        }

        [Test]
        public void Converting_collection_of_StocksTimeSeriesRecords_Should_return_an_collection_of_StockTimeSeries()
        {
            List<StocksTimeSeriesRecord> records = new List<StocksTimeSeriesRecord>();

            records.Add(new StocksTimeSeriesRecord { Datasource = "A", Symbol = "APPL", Data = "APPL Sample data", DateTime = Timestamp.FromDateTime(DateTime.ParseExact("01/02/2010", "dd/MM/yyyy", null).ToUniversalTime()) });
            records.Add(new StocksTimeSeriesRecord { Datasource = "G", Symbol = "GOOG", Data = "GOOG Sample data", DateTime = Timestamp.FromDateTime(DateTime.ParseExact("01/02/2011", "dd/MM/yyyy", null).ToUniversalTime()) });

            var result = _cut.Convert(records);

            result.Count.ShouldBe(2);
            var apple = result.Where(x => x.DataSource == "A").FirstOrDefault();
            var googl = result.Where(x => x.DataSource == "G").FirstOrDefault();

            apple.DataSource.ShouldBe("A");
            apple.Symbol.ShouldBe("APPL");
            apple.Data.ShouldBe("APPL Sample data");
            apple.DateTime.ShouldBe(DateTime.ParseExact("01/02/2010", "dd/MM/yyyy", null).ToUniversalTime());

            googl.DataSource.ShouldBe("G");
            googl.Symbol.ShouldBe("GOOG");
            googl.Data.ShouldBe("GOOG Sample data");
            googl.DateTime.ShouldBe(DateTime.ParseExact("01/02/2011", "dd/MM/yyyy", null).ToUniversalTime());
        }

        [Test]
        public void Converting_collection_of_StocksTimeSeriesRecords_that_is_not_declared_Should_throw_and_exception()
        {
            Assert.Throws<NullReferenceException>(() => _cut.Convert(null));
        }
    }
}
