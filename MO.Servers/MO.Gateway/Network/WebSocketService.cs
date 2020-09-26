using DotNetty.Codecs.Http;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MO.Gateway.Network
{
    public class WebSocketService : IHostedService
    {
        private readonly IClusterClient _client;
        private readonly IConfiguration _config;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        IEventLoopGroup _bossGroup;
        IEventLoopGroup _workerGroup;
        ServerBootstrap _bootstrap = new ServerBootstrap();
        IChannel _bootstrapChannel = null;

        public WebSocketService(
            IClusterClient client,
            IConfiguration config,
            ILoggerFactory loggerFactory)
        {
            _client = client;
            _config = config;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<SocketService>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            IPAddress ip_address = IPAddress.Parse(_config.GetSection("GatewayConfig")["websocket_ip_address"]);
            int port = int.Parse(_config.GetSection("GatewayConfig")["websocket_port"]);
            var dispatcher = new DispatcherEventLoopGroup();
            _bossGroup = dispatcher;
            _workerGroup = new WorkerEventLoopGroup(dispatcher);

            _bootstrap.Group(_bossGroup, _workerGroup);
            _bootstrap.Channel<TcpServerChannel>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _bootstrap
                    .Option(ChannelOption.SoReuseport, true)
                    .ChildOption(ChannelOption.SoReuseaddr, true);
            }

            _bootstrap
            .Option(ChannelOption.SoBacklog, 8192)
            .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
            {
                IChannelPipeline pipeline = channel.Pipeline;
                pipeline.AddLast(new HttpServerCodec());
                pipeline.AddLast(new HttpObjectAggregator(65536));
                pipeline.AddLast(new WebSocketChannelHandler(_client, _loggerFactory, _config));
            }));

            _bootstrapChannel = await _bootstrap.BindAsync(ip_address, port);
            _logger.LogInformation($"Websocket服务启动成功 {ip_address}:{port}");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _bootstrapChannel.CloseAsync();
                _logger.LogInformation($"Websocket服务关闭成功");
            }
            finally
            {
                await Task.WhenAll(
                    _bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    _workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }
    }
}
