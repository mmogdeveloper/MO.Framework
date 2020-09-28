using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MO.Protocol;
using Orleans;
using System;
using System.Text;
using System.Threading.Tasks;
using static DotNetty.Codecs.Http.HttpResponseStatus;
using static DotNetty.Codecs.Http.HttpVersion;

namespace MO.Gateway.Network
{
    public sealed class WebSocketChannelHandler : SimpleChannelInboundHandler<object>
    {
        const string WebsocketPath = "/websocket";
        private readonly ILogger _logger;
        private readonly IClusterClient _client;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;

        private WebSocketServerHandshaker _handshaker;
        private GameSession _session;

        public WebSocketChannelHandler(IClusterClient client, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _client = client;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<SocketChannelHandler>();
            _configuration = configuration;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, object msg)
        {
            if (msg is IFullHttpRequest request)
            {
                this.HandleHttpRequest(ctx, request);
            }
            else if (msg is WebSocketFrame frame)
            {
                this.HandleWebSocketFrame(ctx, frame);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        private void HandleHttpRequest(IChannelHandlerContext ctx, IFullHttpRequest req)
        {
            // Handle a bad request.
            if (!req.Result.IsSuccess)
            {
                SendHttpResponse(ctx, req, new DefaultFullHttpResponse(Http11, BadRequest));
                return;
            }

            // Allow only GET methods.
            if (!Equals(req.Method, HttpMethod.Get))
            {
                SendHttpResponse(ctx, req, new DefaultFullHttpResponse(Http11, Forbidden));
                return;
            }

            // Send the demo page and favicon.ico
            if ("/".Equals(req.Uri))
            {
                var res = new DefaultFullHttpResponse(Http11, NotFound);
                SendHttpResponse(ctx, req, res);
                return;
            }

            if ("/favicon.ico".Equals(req.Uri))
            {
                var res = new DefaultFullHttpResponse(Http11, NotFound);
                SendHttpResponse(ctx, req, res);
                return;
            }

            // Handshake
            var wsFactory = new WebSocketServerHandshakerFactory(
                GetWebSocketLocation(req), null, true, 5 * 1024 * 1024);
            this._handshaker = wsFactory.NewHandshaker(req);
            if (this._handshaker == null)
            {
                WebSocketServerHandshakerFactory.SendUnsupportedVersionResponse(ctx.Channel);
            }
            else
            {
                this._handshaker.HandshakeAsync(ctx.Channel, req);
            }
        }

        private async void HandleWebSocketFrame(IChannelHandlerContext ctx, WebSocketFrame frame)
        {
            try
            {
                // Check for closing frame
                if (frame is CloseWebSocketFrame)
                {
                    await _handshaker.CloseAsync(ctx.Channel, (CloseWebSocketFrame)frame.Retain());
                    return;
                }

                if (frame is PingWebSocketFrame)
                {
                    await ctx.WriteAsync(new PongWebSocketFrame((IByteBuffer)frame.Content.Retain()));
                    return;
                }

                if (frame is TextWebSocketFrame)
                {
                    // Echo the frame
                    var revBuffer = frame.Content as IByteBuffer;
                    var dataBuffer = new byte[revBuffer.ReadableBytes];
                    revBuffer.ReadBytes(dataBuffer);
                    var msg = MOMsg.Parser.ParseFrom(ByteString.FromBase64(Encoding.UTF8.GetString(dataBuffer)));
                    if (msg != null)
                    {
                        await _session.DispatchIncomingPacket(msg);
                    }
                    return;
                }

                if (frame is BinaryWebSocketFrame)
                {
                    // Echo the frame
                    var revBuffer = frame.Content as IByteBuffer;
                    var dataBuffer = new byte[revBuffer.ReadableBytes];
                    revBuffer.ReadBytes(dataBuffer);
                    var msg = MOMsg.Parser.ParseFrom(dataBuffer);
                    if (msg != null)
                    {
                        await _session.DispatchIncomingPacket(msg);
                    }
                    return;
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

        static void SendHttpResponse(IChannelHandlerContext ctx, IFullHttpRequest req, IFullHttpResponse res)
        {
            // Generate an error page if response getStatus code is not OK (200).
            if (res.Status.Code != 200)
            {
                IByteBuffer buf = Unpooled.CopiedBuffer(Encoding.UTF8.GetBytes(res.Status.ToString()));
                res.Content.WriteBytes(buf);
                buf.Release();
                HttpUtil.SetContentLength(res, res.Content.ReadableBytes);
            }

            // Send the response and close the connection if necessary.
            Task task = ctx.Channel.WriteAndFlushAsync(res);
            if (!HttpUtil.IsKeepAlive(req) || res.Status.Code != 200)
            {
                task.ContinueWith((t, c) => ((IChannelHandlerContext)c).CloseAsync(),
                    ctx, TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext ctx, Exception e)
        {
            ctx.CloseAsync();
        }

        static string GetWebSocketLocation(IFullHttpRequest req)
        {
            bool result = req.Headers.TryGet(HttpHeaderNames.Host, out ICharSequence value);
            string location = value.ToString() + WebsocketPath;
            return "ws://" + location;
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            _session = new GameSession(_client, _loggerFactory, _configuration, context);
            base.ChannelRegistered(context);
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            _session.Disconnect();
            base.ChannelUnregistered(context);
        }
    }
}
