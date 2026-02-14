using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MO.GrainInterfaces;
using MO.Model.Context;
using NLog.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.IO;

namespace MO.Silo
{
    class Program
    {
        public static IConfiguration Configuration { get; private set; }
        static void Main(string[] args)
        {
            Configuration = LoadConfiguration();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(builder =>
                {
                    builder.AddNLog();
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<DBTaskService>();

                    var moDataConn = Configuration.GetConnectionString("MOData");
                    services.AddDbContext<MODataContext>(options =>
                    {
                        options.UseMySql(moDataConn, ServerVersion.AutoDetect(moDataConn));
                    });

                    var moRecordConn = Configuration.GetConnectionString("MORecord");
                    services.AddDbContext<MORecordContext>(options =>
                    {
                        options.UseMySql(moRecordConn, ServerVersion.AutoDetect(moRecordConn));
                    });

                    services.AddSingleton<MODataContext>();
                    services.AddSingleton<MORecordContext>();

                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .UseOrleans(builder =>
                {
                    builder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = Configuration.GetSection("ClusterConfig")["ClusterId"];
                        options.ServiceId = Configuration.GetSection("ClusterConfig")["ServiceId"];
                    })
                    .Configure<HostOptions>(options =>
                    {
                        options.ShutdownTimeout = TimeSpan.FromMinutes(1);
                    })
                    .AddMemoryStreams(StreamProviders.JobsProvider)
                    .AddMemoryStreams(StreamProviders.TransientProvider)
                    .ConfigureEndpoints(
                        hostname: Configuration.GetSection("ClusterConfig")["HostName"],
                        siloPort: UInt16.Parse(Configuration.GetSection("ClusterConfig")["SiloPort"]),
                        gatewayPort: UInt16.Parse(Configuration.GetSection("ClusterConfig")["GatewayPort"]))
                    .UseAdoNetClustering(options =>
                    {
                        options.Invariant = Configuration.GetSection("ClusterConfig")["Invariant"];
                        options.ConnectionString = Configuration.GetSection("ClusterConfig")["ConnectionString"];
                    })
                    .UseAdoNetReminderService(options =>
                    {
                        options.Invariant = Configuration.GetSection("ReminderConfig")["Invariant"];
                        options.ConnectionString = Configuration.GetSection("ReminderConfig")["ConnectionString"];
                    })
                    .AddAdoNetGrainStorage(StorageProviders.DefaultProviderName, options =>
                    {
                        options.Invariant = Configuration.GetSection("StorageConfig")["Invariant"];
                        options.ConnectionString = Configuration.GetSection("StorageConfig")["ConnectionString"];
                    })
                    .AddAdoNetGrainStorage("PubSubStore", options =>
                    {
                        options.Invariant = Configuration.GetSection("StorageConfig")["Invariant"];
                        options.ConnectionString = Configuration.GetSection("StorageConfig")["ConnectionString"];
                    })
                    .UseTransactions();
                });
        }

        private static IConfiguration LoadConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", false, false);
            return configurationBuilder.Build();
        }
    }
}
