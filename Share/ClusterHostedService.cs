using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MO.GrainInterfaces;
using Orleans.Configuration;
using Orleans.Hosting;

namespace MO
{
    public static class OrleansClientExtensions
    {
        public static IHostBuilder UseOrleansClusterClient(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseOrleansClient((context, clientBuilder) =>
            {
                var config = context.Configuration;
                clientBuilder
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = config.GetSection("ClusterConfig")["ClusterId"];
                        options.ServiceId = config.GetSection("ClusterConfig")["ServiceId"];
                    })
                    .UseAdoNetClustering(options =>
                    {
                        options.Invariant = config.GetSection("ClusterConfig")["Invariant"];
                        options.ConnectionString = config.GetSection("ClusterConfig")["ConnectionString"];
                    })
                    .AddMemoryStreams(StreamProviders.JobsProvider)
                    .AddMemoryStreams(StreamProviders.TransientProvider);
            });
        }
    }
}
