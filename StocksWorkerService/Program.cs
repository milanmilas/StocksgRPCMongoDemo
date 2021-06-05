using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using StocksWorkerService.Configurations;
using StocksWorkerService.Services.Alphavantage;

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

                    services.AddHostedService<Worker>();

                    var alphavantageServiceConfiguration = configuration
                        .GetSection(nameof(AlphavantageServiceConfiguration))
                        .Get<AlphavantageServiceConfiguration>();
                    services.AddSingleton(alphavantageServiceConfiguration);
                    services.AddSingleton<IAlphavantageService, AlphavantageService>();
                });
    }
}
