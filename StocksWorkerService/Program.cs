using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using StocksWorkerService.Configurations;
using StocksWorkerService.Services.Alphavantage;
using System.Net.Http;
using Polly.Extensions.Http;
using Polly.Timeout;
using Polly;

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
