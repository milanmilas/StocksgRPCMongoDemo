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

                    var retryPolicy = HttpPolicyExtensions
                          .HandleTransientHttpError()
                          .Or<TimeoutRejectedException>()
                          .RetryAsync(5);

                    var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(30);

                    services.AddHttpClient("retryclient")
                        .AddPolicyHandler(retryPolicy)
                        .AddPolicyHandler(timeoutPolicy);

                    services.AddHostedService<Worker>();

                    var alphavantageServiceConfiguration = configuration
                        .GetSection(nameof(AlphavantageServiceConfiguration))
                        .Get<AlphavantageServiceConfiguration>();
                    services.AddSingleton(alphavantageServiceConfiguration);
                    services.AddSingleton<IAlphavantageService, AlphavantageService>();
                    
                });
    }
}
