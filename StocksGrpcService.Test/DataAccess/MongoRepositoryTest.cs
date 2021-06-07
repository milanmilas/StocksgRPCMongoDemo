using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using StocksGrpcService.DataAccess;
using StocksGrpcService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksGrpcService.Test.DataAccess
{
    [TestFixture]
    class MongoRepositoryTest
    {
        private ILogger<MongoRepository<StockTimeSeries>> _logger;
        private Mock<IMongoDBContext<StockTimeSeries>> _dBContext;
        private MongoRepository<StockTimeSeries> _cut;

        private Mock<IMongoCollection<StockTimeSeries>> _collection;

        [SetUp]
        public void Setup()
        {
            _logger = Mock.Of<ILogger<MongoRepository<StockTimeSeries>>>();
            _collection = new Mock<IMongoCollection<StockTimeSeries>>();
            _dBContext = new Mock<IMongoDBContext<StockTimeSeries>>();
            _dBContext.Setup(x => x.GetCollection("stockstimeseries")).Returns(_collection.Object);

            _cut = new MongoRepository<StockTimeSeries>(_logger, _dBContext.Object);

        }

        [Test]
        public void When_constructing_MongoRepository_of_StockTimeSeries_Type_Then_collestion_with_name_stockstimeseries_should_be_requested()
        {
            _dBContext.VerifyAll();
        }

        [Test]
        public void When_inserting_many_documents_then_all_documents_should_be_inserted_into_collection()
        {
            var items = new List<StockTimeSeries>             {
                new StockTimeSeries(),
                new StockTimeSeries()
            };

            _cut.InsertMany(items);

            _collection.Verify(x => x.InsertMany(items, null, default), Times.Once);
        }

        [Test]
        public async Task When_inserting_many_documents_asynchronous_then_all_documents_should_be_inserted_into_collection()
        {
            var items = new List<StockTimeSeries>             {
                new StockTimeSeries(),
                new StockTimeSeries()
            };

            await _cut.InsertManyAsync(items);

            _collection.Verify(x => x.InsertManyAsync(items, null, default), Times.Once);
        }
    }
}
