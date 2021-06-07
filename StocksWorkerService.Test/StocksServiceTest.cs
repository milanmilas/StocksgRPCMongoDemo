using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Shouldly;
using StocksWorkerService.Configurations;
using StocksWorkerService.Services;
using StocksWorkerService.Services.Alphavantage;
using StocksWorkerService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StocksWorkerService.Test
{
    [TestFixture]
    public class StocksServiceTest
    {

        private ILogger<StocksService<AlphavantageServiceConfiguration>> _logger;
        WebApiConfiguration _config;
        private Mock<IUrlBuilder<AlphavantageServiceConfiguration>> _urlBuilder;
        private Mock<IHttpClientFactory> _httpClientFactory;
        private Mock<IDateTime> _dateTime;
        private StocksService<AlphavantageServiceConfiguration> _cut;
        private Mock<HttpMessageHandler> _httpMessageHandler;

        [SetUp]
        public void Setup()
        {
            _logger = Mock.Of<ILogger<StocksService<AlphavantageServiceConfiguration>>>();
            _config = new WebApiConfiguration { ParallelRequests = 10 };
            _urlBuilder = new Mock<IUrlBuilder<AlphavantageServiceConfiguration>>();
            _urlBuilder.SetupGet(d => d.DataSource).Returns("Test DataSource");
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _dateTime = new Mock<IDateTime>();
            _dateTime.Setup(x => x.UtcNow())
                .Returns(DateTime.ParseExact("07/02/2010", "dd/MM/yyyy", null).ToUniversalTime());

            _cut = new StocksService<AlphavantageServiceConfiguration>(
                _logger,
                _config,
                _urlBuilder.Object,
                _httpClientFactory.Object,
                _dateTime.Object);

            _httpMessageHandler = new Mock<HttpMessageHandler>();
            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{'name':test}"),
                });
        }

        [Test]
        public void StocksService_constructor_test()
        {
        }

        [Test]
        public async Task Requesting_stocks_Should_use_retryclient_for_http_requests()
        {
            _urlBuilder.Setup(x => x.BuildSymbolsUrls()).Returns(new List<(string, string)>());
            _httpClientFactory.Setup(x => x.CreateClient("retryclient")).Returns(new HttpClient(_httpMessageHandler.Object));

            await _cut.GetStocks();

            _httpClientFactory.VerifyAll();
        }

        [Test]
        public async Task Requesting_stocks_Should_use_http_request_urls_from_url_builder_when_making_calls_to_get_stocks_data()
        {
            _urlBuilder.Setup(x => x.BuildSymbolsUrls()).Returns(new List<(string, string)>
            {
                ("APPL", "http://appl"),
                ("GOOG", "http://goog")
            });

            List<HttpRequestMessage> httpRequestMessages = new List<HttpRequestMessage>();

            int resonseCounter = 0;
            _httpMessageHandler = new Mock<HttpMessageHandler>();
            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>(
                    (_httpRequestMessage, _cancellationToken) =>
                    {
                        httpRequestMessages.Add(_httpRequestMessage);
                    })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"{{'name':test{resonseCounter++}}}"),
                });

            _httpClientFactory.Setup(x => x.CreateClient("retryclient")).Returns(new HttpClient(_httpMessageHandler.Object));

            await _cut.GetStocks();

            httpRequestMessages.Count().ShouldBe(2);
            httpRequestMessages.Where(x => x.RequestUri.ToString() == "http://appl").Count().ShouldBe(1);
            httpRequestMessages.Where(x => x.RequestUri.ToString() == "http://goog").Count().ShouldBe(1);
        }

        [Test]
        public async Task Requesting_stocks_Should_should_return_stocks_with_valid_fields_for_each_symbol()
        {
            _urlBuilder.Setup(x => x.BuildSymbolsUrls()).Returns(new List<(string, string)>
            {
                ("APPL", "http://appl"),
                ("GOOG", "http://goog")
            });


            int resonseCounter = 0;
            _httpMessageHandler = new Mock<HttpMessageHandler>();
            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => {
                    var val = Interlocked.Increment(ref resonseCounter);
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent($"{{'name':test{val}}}"),
                    };
                    });

            _httpClientFactory.Setup(x => x.CreateClient("retryclient")).Returns(new HttpClient(_httpMessageHandler.Object));

            var result = await _cut.GetStocks();

            result.Count.ShouldBe(2);

            result.Count.ShouldBe(2);
            var response1 = result.Where(x => x.Data == "{'name':test1}").FirstOrDefault();
            var response2 = result.Where(x => x.Data == "{'name':test2}").FirstOrDefault();

            response1.DataSource.ShouldBe("Test DataSource");
            response1.Data.ShouldBe("{'name':test1}");
            response1.DateTime.ShouldBe(DateTime.ParseExact("07/02/2010", "dd/MM/yyyy", null).ToUniversalTime());

            response2.DataSource.ShouldBe("Test DataSource");
            response2.Data.ShouldBe("{'name':test2}");
            response2.DateTime.ShouldBe(DateTime.ParseExact("07/02/2010", "dd/MM/yyyy", null).ToUniversalTime());

            result.Where(x => x.Symbol == "APPL").Count().ShouldBe(1);
            result.Where(x => x.Symbol == "GOOG").Count().ShouldBe(1);
        }
    }
}