using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MO.GrainInterfaces;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MO.Api
{
    public class ClusterHostedService : IHostedService
    {
        private readonly ILogger<ClusterHostedService> _logger;
        private readonly IConfiguration _config;

        public ClusterHostedService(ILogger<ClusterHostedService> logger, ILoggerProvider loggerProvider,IConfiguration config)
        {
            _logger = logger;
            _config = config;

            Client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = _config.GetSection("ClusterConfig")["ClusterId"];
                    options.ServiceId = _config.GetSection("ClusterConfig")["ServiceId"];
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
                //.ConfigureServices()
                .ConfigureLogging(builder => builder.AddProvider(loggerProvider))
                .ConfigureApplicationParts(parts => { parts.AddFromApplicationBaseDirectory().WithReferences(); })
                .AddSimpleMessageStreamProvider(StreamProviders.JobsProvider)
                .AddSimpleMessageStreamProvider(StreamProviders.TransientProvider)
                //.UseLocalhostClustering()
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = _config.GetSection("ClusterConfig")["Invariant"];
                    options.ConnectionString = _config.GetSection("ClusterConfig")["ConnectionString"];
                })
                .Build();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var attempt = 0;
            var maxAttempts = 100;
            var delay = TimeSpan.FromSeconds(1);
            return Client.Connect(async error =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                if (++attempt < maxAttempts)
                {
                    _logger.LogWarning(error,
                        "Failed to connect to Orleans cluster on attempt {@Attempt} of {@MaxAttempts}.",
                        attempt, maxAttempts);

                    try
                    {
                        await Task.Delay(delay, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    _logger.LogError(error,
                        "Failed to connect to Orleans cluster on attempt {@Attempt} of {@MaxAttempts}.",
                        attempt, maxAttempts);

                    return false;
                }
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Client.Close();
            }
            catch (OrleansException error)
            {
                _logger.LogWarning(error, "Error while gracefully disconnecting from Orleans cluster. Will ignore and continue to shutdown.");
            }
        }

        public IClusterClient Client { get; }
    }
}
