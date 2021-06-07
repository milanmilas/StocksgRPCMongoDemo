using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using StocksGrpcService.DataAccess;
using StocksGrpcService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;

namespace StocksGrpcService.Test.Services
{
    [TestFixture]
    public class StocksTimeSeriesServiceTest
    {
        private ILogger<GreeterService> _logger;
        private Mock<IMongoRepository<StockTimeSeries>> _repository;
        private Mock<IConverter<StocksTimeSeriesRecord, StockTimeSeries>> _converter;

        private StocksTimeSeriesService _cut;

        [SetUp]
        public void Setup()
        {
            _logger = Mock.Of<ILogger<GreeterService>>();
            _repository = new Mock<IMongoRepository<StockTimeSeries>>();
            _converter = new Mock<IConverter<StocksTimeSeriesRecord, StockTimeSeries>>();

            _cut = new StocksTimeSeriesService(_logger, _repository.Object, _converter.Object);
        }

        [Test]
        public async Task When_a_Create_request_is_received_Then_converter_should_be_called_for_the_request_data()
        {
            var request = new StocksTimeSeriesCreateRequest();
            request
                .StocksTimeSeries
                .Add(new StocksTimeSeriesRecord { Symbol = "GOOG", DateTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow) });

            await _cut.Create(request, null);

            _converter.Verify(x => x.Convert(request.StocksTimeSeries), Times.Once);
        }

        [Test]
        public async Task When_a_Create_request_is_received_Then_stock_time_series_data_should_be_persisted()
        {
            var convertedTimeSeries = new List<StockTimeSeries>();

            _converter
                .Setup(x => x.Convert(It.IsAny<IEnumerable<StocksTimeSeriesRecord>>()))
                .Returns(convertedTimeSeries);

            var request = new StocksTimeSeriesCreateRequest();

            await _cut.Create(request, null);

            _repository.Verify(x => x.InsertManyAsync(convertedTimeSeries), Times.Once);
        }

        [Test]
        public async Task When_a_Create_request_is_received_Then_OK_response_should_be_returned_on_success()
        {

            var request = new StocksTimeSeriesCreateRequest();

            var response = await _cut.Create(request, null);

            response.Status.ShouldBe(StatusCode.Ok);
        }

        [Test]
        public async Task When_a_Create_request_is_received_Then_Error_response_should_be_returned_on_error()
        {
            _converter
               .Setup(x => x.Convert(It.IsAny<IEnumerable<StocksTimeSeriesRecord>>()))
               .Throws(new Exception());

            var request = new StocksTimeSeriesCreateRequest();

            var response = await _cut.Create(request, null);

            response.Status.ShouldBe(StatusCode.Error);
        }
    }
}
