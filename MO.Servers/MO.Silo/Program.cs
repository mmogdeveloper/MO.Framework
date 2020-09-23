using CSRedis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MO.Algorithm.Redis;
using MO.GrainInterfaces;
using MO.Model.Context;
using NLog.Extensions.Logging;
using Orleans;
using Orleans.ApplicationParts;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Serialization;
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
                    //builder.AddConsole();//可替换成NLog
                    builder.ClearProviders();
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddNLog();
                })
                .ConfigureServices(services =>
                {
                    services.AddDbContext<MODataContext>(options =>
                    {
                        options.UseMySql(Configuration.GetConnectionString("MOData"));
                    });
                    services.AddDbContext<MORecordContext>(options =>
                    {
                        options.UseMySql(Configuration.GetConnectionString("MORecord"));
                    });

                    //初始化数据库
                    services.BuildServiceProvider().GetService<MODataContext>().Database.Migrate();
                    services.BuildServiceProvider().GetService<MORecordContext>().Database.Migrate();

                    DataRedis.Initialization(Configuration.GetConnectionString("DataRedis"));
                    //CSRedisClient redisClient = new CSRedisClient(() =>);
                    //RedisHelper.Initialization(redisClient);

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
                    .Configure<SchedulingOptions>(options =>
                    {
                        options.AllowCallChainReentrancy = true;
                        options.PerformDeadlockDetection = true;
                    })
                    .Configure<SerializationProviderOptions>(options =>
                    {
                        options.SerializationProviders.Add(typeof(ProtobufSerializer));
                        options.FallbackSerializationProvider = typeof(ProtobufSerializer);
                    })
                    .AddSimpleMessageStreamProvider(StreamProviders.JobsProvider)
                    .AddSimpleMessageStreamProvider(StreamProviders.TransientProvider)

                    .ConfigureApplicationParts(parts =>
                    {
                        parts.AddFromApplicationBaseDirectory().WithReferences();
                    })
                    //.ConfigureServices(ConfigureServices)
                    //.ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                    //.UseLocalhostClustering()
                    .ConfigureEndpoints(
                    hostname: Configuration.GetSection("ClusterConfig")["HostName"],
                    siloPort: UInt16.Parse(Configuration.GetSection("ClusterConfig")["SiloPort"]),
                    gatewayPort: UInt16.Parse(Configuration.GetSection("ClusterConfig")["GatewayPort"]))
                    //.ConfigureEndpoints(siloPort: 11112, gatewayPort: 30001)
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
                        options.UseJsonFormat = true;
                    })
                    .AddAdoNetGrainStorage("PubSubStore", options =>
                    {
                        options.Invariant = Configuration.GetSection("StorageConfig")["Invariant"];
                        options.ConnectionString = Configuration.GetSection("StorageConfig")["ConnectionString"];
                        options.UseJsonFormat = true;
                    });
                });
        }

        #region config

        private static IConfiguration LoadConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", false, false);
            return configurationBuilder.Build();
        }
        #endregion
    }
}
