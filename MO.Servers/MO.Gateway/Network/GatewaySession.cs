using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MO.Algorithm;
using MO.Algorithm.Actions.Enum;
using MO.Algorithm.Redis;
using MO.Common.Config;
using MO.Common.Security;
using MO.GrainInterfaces.Network;
using MO.GrainInterfaces.User;
using Newtonsoft.Json;
using Orleans;
using ProtoMessage;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MO.Gateway.Network
{
    public class GatewaySession
    {
        private readonly IClusterClient _client;
        private readonly ILogger _logger;
        private readonly Guid _sessionId;
        private readonly IConfiguration _configuration;

        private OutcomingPacketObserver _packetObserver;
        private IPacketObserver _packetObserverRef;
        private IChannelHandlerContext _context;
        private IPacketRouter _router;
        private IUser _userGrain;
        private bool _IsInit;
        private long _userId;
        private string _token;

        public GatewaySession(IClusterClient client, ILoggerFactory loggerFactory,
            IConfiguration configuration, IChannelHandlerContext context)
        {
            _client = client;
            _logger = loggerFactory.CreateLogger<GatewaySession>();
            _configuration = configuration;
            _context = context;
            _sessionId = Guid.NewGuid();
        }

        public void Disconnect()
        {
            if (_router != null)
                _router.Disconnect();
        }

        public async Task DispatchIncomingPacket(MOMsg packet)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Restart();
                //md5签名验证
                var key = _configuration.GetValue<string>("MD5Key");
                var sign = packet.Sign;
                packet.Sign = string.Empty;
                var data = packet.ToByteString();
                if (CryptoHelper.MD5_Encrypt($"{data}{key}").ToLower() != sign.ToLower())
                {
                    await DispatchOutcomingPacket(packet.ParseResult(ErrorType.Hidden, "签名验证失败"));
                    await Close();
                    return;
                }

                //同步初始化
                if (!_IsInit)
                {
                    _userGrain = _client.GetGrain<IUser>(packet.UserId);
                    var tokenInfo = await _userGrain.GetToken();
                    if (tokenInfo.Token != packet.Token || tokenInfo.LastTime.AddSeconds(30) < DateTime.Now)
                    {
                        await DispatchOutcomingPacket(packet.ParseResult(ErrorType.Hidden, "Token验证失败"));
                        await Close();
                        return;
                    }
                    _userId = packet.UserId;
                    _token = tokenInfo.Token;
                    _packetObserver = new OutcomingPacketObserver(this);
                    _router = _client.GetGrain<IPacketRouter>(_sessionId);
                    _packetObserverRef = _client.CreateObjectReference<IPacketObserver>(_packetObserver).Result;
                    _router.SetObserver(_packetObserverRef).Wait();
                    _IsInit = true;
                }
                else
                {
                    if (_userId != packet.UserId || _token != packet.Token)
                    {
                        await DispatchOutcomingPacket(packet.ParseResult(ErrorType.Hidden, "Token验证失败"));
                        await Close();
                        return;
                    }
                }

                //心跳包
                if (packet.ActionId == 1)
                {
                    await _userGrain.RefreshTokenTime();
                    return;
                }

                await _router.SendPacket(packet);

                watch.Stop();
                Console.WriteLine($"{packet.UserId} {watch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"DispatchIncomingPacket异常:\n" +
                    $"{ex.Message}\n" +
                    $"{ex.StackTrace}\n" +
                    $"{JsonConvert.SerializeObject(packet)}");
            }
        }

        public async Task DispatchOutcomingPacket(MOMsg packet)
        {
            try
            {
                if (_context.Channel.Active)
                {
                    var bytes = packet.ToByteArray();
                    IByteBuffer buffer = Unpooled.WrappedBuffer(bytes);
                    await _context.WriteAndFlushAsync(buffer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"DispatchOutcomingPacket异常:\n" +
                    $"{ex.Message}\n" +
                    $"{ex.StackTrace}\n" +
                    $"{JsonConvert.SerializeObject(packet)}");
            }
        }

        public async Task Close()
        {
            await _context.CloseAsync();
        }

        class OutcomingPacketObserver : IPacketObserver
        {
            private readonly GatewaySession session;

            public OutcomingPacketObserver(GatewaySession session)
            {
                this.session = session;
            }

            public async void Close(MOMsg packet = null)
            {
                if (packet != null)
                    await session.DispatchOutcomingPacket(packet);
                await session.Close();
            }

            public async void SendPacket(MOMsg packet)
            {
                await session.DispatchOutcomingPacket(packet);
            }
        }
    }
}
