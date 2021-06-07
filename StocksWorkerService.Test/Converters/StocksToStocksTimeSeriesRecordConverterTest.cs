using Moq;
using NUnit.Framework;
using Shouldly;
using StocksWorkerService.Converters;
using StocksWorkerService.Model;
using StocksWorkerService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;

namespace StocksWorkerService.Test.Converters
{
    [TestFixture]
    public class StocksToStocksTimeSeriesRecordConverterTest
    {
        private Mock<IDateTime> _dateTime;

        private StocksToStocksTimeSeriesRecordConverter _cut;

        [SetUp]
        public void Setup()
        {
            _dateTime = new Mock<IDateTime>();
            _dateTime.Setup(x => x.UtcNow())
                .Returns(DateTime.ParseExact("07/02/2010", "dd/MM/yyyy", null).ToUniversalTime());

            _cut = new StocksToStocksTimeSeriesRecordConverter(_dateTime.Object);
        }

        [Test]
        public void Converting_empty_collection_of_Stocks_Should_return_an_emtpy_collection_of_StocksTimeSeriesRecord()
        {
            List<Stock> records = new List<Stock>();
            var result = _cut.Convert(records);

            result.ShouldBeEmpty();
        }

        [Test]
        public void Converting_collection_of_Stocks_Should_return_an_collection_of_StocksTimeSeriesRecords()
        {
            List<Stock> records = new List<Stock>
            {
                new Stock{ DataSource = "A", Symbol = "APPL", Data = "APPL Sample data", DateTime = DateTime.ParseExact("01/02/2010", "dd/MM/yyyy", null).ToUniversalTime()},
                new Stock{ DataSource = "G", Symbol = "GOOG", Data = "GOOG Sample data", DateTime = DateTime.ParseExact("01/02/2011", "dd/MM/yyyy", null).ToUniversalTime()},
            };

            var result = _cut.Convert(records);

            result.Count.ShouldBe(2);
            var apple = result.Where(x => x.Datasource == "A").FirstOrDefault();
            var googl = result.Where(x => x.Datasource == "G").FirstOrDefault();

            apple.Datasource.ShouldBe("A");
            apple.Symbol.ShouldBe("APPL");
            apple.Data.ShouldBe("APPL Sample data");
            apple.DateTime.ShouldBe(Timestamp.FromDateTime(DateTime.ParseExact("01/02/2010", "dd/MM/yyyy", null).ToUniversalTime()));
            apple.Timestamp.ShouldBe(Timestamp.FromDateTime(DateTime.ParseExact("07/02/2010", "dd/MM/yyyy", null).ToUniversalTime()));

            googl.Datasource.ShouldBe("G");
            googl.Symbol.ShouldBe("GOOG");
            googl.Data.ShouldBe("GOOG Sample data");
            googl.DateTime.ShouldBe(Timestamp.FromDateTime(DateTime.ParseExact("01/02/2011", "dd/MM/yyyy", null).ToUniversalTime()));
            googl.Timestamp.ShouldBe(Timestamp.FromDateTime(DateTime.ParseExact("07/02/2010", "dd/MM/yyyy", null).ToUniversalTime()));
        }

        [Test]
        public void Converting_collection_of_Stocks_that_is_not_declared_Should_throw_and_exception()
        {
            Assert.Throws<NullReferenceException>(() => _cut.Convert(null));
        }
    }
}
