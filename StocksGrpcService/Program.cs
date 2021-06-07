using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StocksGrpcService.Configurations;
using StocksGrpcService.DataAccess;
using StocksGrpcService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StocksGrpcService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    var databaseConfiguration = configuration
                        .GetSection(nameof(DatabaseConfiguration))
                        .Get<DatabaseConfiguration>();
                    services.AddSingleton(databaseConfiguration);

                    services.AddSingleton<IMongoDBContext<StockTimeSeries>, MongoDBContext<StockTimeSeries>>();
                    // MongoClient is thread safe, meke it transient if any issues arrise
                    services.AddSingleton<IMongoRepository<StockTimeSeries>, MongoRepository<StockTimeSeries>>();
                });
    }
}
