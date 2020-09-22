using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MO.Gateway.Network;
using NLog.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using System.IO;

namespace MO.Gateway
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ClusterHostedService>();
                    services.AddSingleton<IHostedService>(_ => _.GetService<ClusterHostedService>());
                    services.AddSingleton(_ => _.GetService<ClusterHostedService>().Client);

                    services.AddHostedService<GatewayService>();
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureAppConfiguration(builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json", false, false);
                })
                .ConfigureLogging(builder =>
                {
                    //builder.AddConsole();
                    builder.ClearProviders();
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddNLog();
                });
        }
    }
}
