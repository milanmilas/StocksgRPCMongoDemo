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
                          .RetryAsync(webApiConfiguration.RetryAttempts);

                    var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(webApiConfiguration.RequestTimeoutSeconds);

                    services.AddHttpClient("retryclient")
                        .AddPolicyHandler(retryPolicy)
                        .AddPolicyHandler(timeoutPolicy);

                    var alphavantageServiceConfiguration = configuration
                       .GetSection(nameof(AlphavantageServiceConfiguration))
                       .Get<AlphavantageServiceConfiguration>();
                    services.AddSingleton(alphavantageServiceConfiguration);
                    services.AddSingleton<IUrlBuilder, AlphaventageUrlBuilder>();
                    services.AddSingleton<IAlphavantageService, AlphavantageService>();

                    services.AddHostedService<Worker>();
                });
    }
}
