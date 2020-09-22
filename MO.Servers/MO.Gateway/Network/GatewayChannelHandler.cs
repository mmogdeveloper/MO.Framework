using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MO.Gateway.Network
{
    public class GatewayChannelHandler : ChannelHandlerAdapter
    {
        private readonly ILogger _logger;
        private readonly IClusterClient _client;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;
        private GatewaySession _session;
        public GatewayChannelHandler(IClusterClient client, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _client = client;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<GatewayChannelHandler>();
            _configuration = configuration;
        }

        public override async void ChannelActive(IChannelHandlerContext context)
        {
            await _session.Startup();
        }

        public override async void ChannelRead(IChannelHandlerContext context, object message)
        {
            try
            {
                var revBuffer = message as IByteBuffer;
                var dataBuffer = new byte[revBuffer.ReadableBytes];
                revBuffer.ReadBytes(dataBuffer);
                var msg = MOMsg.Parser.ParseFrom(dataBuffer);
                if (msg != null)
                {
                    await _session.DispatchIncomingPacket(msg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"ChannelRead Exception:\n" +
                    $"{ex.Message}\n" +
                    $"{ex.StackTrace}");
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _logger.LogError($"Gateway Exception: {exception}");
            context.CloseAsync();
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            _session = new GatewaySession(_client, _loggerFactory, _configuration, context);
            base.ChannelRegistered(context);
        }

        public override async void ChannelUnregistered(IChannelHandlerContext context)
        {
            await _session.Disconnect();
            base.ChannelUnregistered(context);
        }
    }
}
