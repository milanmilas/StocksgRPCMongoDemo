using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StocksWorkerService.Configurations;
using StocksWorkerService.Services.Alphavantage;
using System.Net.Http;
using Polly.Extensions.Http;
using Polly.Timeout;
using Polly;
using NLog.Web;
using StocksWorkerService.Services;
using StocksWorkerService.Utils;
using StocksGrpcService;
using StocksWorkerService.Model;
using StocksWorkerService.Converters;
using StocksWorkerService.Schedulers;

namespace StocksWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                })
                .UseNLog()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    var webApiConfiguration = configuration
                        .GetSection(nameof(WebApiConfiguration))
                        .Get<WebApiConfiguration>();
                    services.AddSingleton(webApiConfiguration);

                    var retryPolicy = HttpPolicyExtensions
                          .HandleTransientHttpError()
                          .Or<TimeoutRejectedException>()
                          .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                          .RetryAsync(webApiConfiguration.RetryAttempts);

                    var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(webApiConfiguration.RequestTimeoutSeconds);

                    services.AddHttpClient("retryclient")
                        .AddPolicyHandler(retryPolicy)
                        .AddPolicyHandler(timeoutPolicy);

                    var stocksTimeSeriesClientConfiguration = configuration.RegisterConfiguration<StocksTimeSeriesClientConfiguration>();
                    services.AddSingleton(stocksTimeSeriesClientConfiguration);
                    services.AddSingleton<IDateTime, DateTimeWrapper>();
                    services.AddTransient<IStocksTimeSeriesClientWrapper, StocksTimeSeriesClientWrapper>();
                    services.AddSingleton<IConverter<Stock, StocksTimeSeriesRecord>, StocksToStocksTimeSeriesRecordConverter>();
                        

                    var alphavantageServiceConfiguration = configuration.RegisterConfiguration<AlphavantageServiceConfiguration>();
                    services.AddSingleton(alphavantageServiceConfiguration);
                    services.AddSingleton<IUrlBuilder<AlphavantageServiceConfiguration>, AlphaventageUrlBuilder>();
                    services.AddTransient<IStocksService<AlphavantageServiceConfiguration>, StocksService<AlphavantageServiceConfiguration>>();

                    services.AddTransient<IStocksFeeder<AlphavantageServiceConfiguration>, StocksFeeder<AlphavantageServiceConfiguration>>();
                    services.AddTransient<IScheduler, Scheduler<AlphavantageServiceConfiguration>>();

                    var iexServiceConfiguration = configuration.RegisterConfiguration<IexServiceConfiguration>();
                    services.AddSingleton(iexServiceConfiguration);
                    services.AddSingleton<IUrlBuilder<IexServiceConfiguration>, IexUrlBuilder>();
                    services.AddTransient<IStocksService<IexServiceConfiguration>, StocksService<IexServiceConfiguration>>();

                    services.AddTransient<IStocksFeeder<IexServiceConfiguration>, StocksFeeder<IexServiceConfiguration>>();
                    services.AddTransient<IScheduler, Scheduler<IexServiceConfiguration>>();

                    services.AddHostedService<Worker>();
                });
    }
}
