using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MO.Algorithm;
using MO.Algorithm.Enum;
using MO.GrainInterfaces.Config;
using MO.Common.Security;
using MO.GrainInterfaces.Network;
using MO.GrainInterfaces.User;
using MO.Protocol;
using Newtonsoft.Json;
using Orleans;
using System;
using System.Threading.Tasks;
using MO.GrainInterfaces.Extensions;

namespace MO.Gateway.Network
{
    public class GameSession
    {
        private readonly IClusterClient _client;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        private OutcomingPacketObserver _packetObserver;
        private IPacketObserver _packetObserverRef;
        private IChannelHandlerContext _context;
        private IPacketRouterGrain _router;
        private IUserGrain _user;
        private ITokenGrain _tokenGrain;
        private bool _IsInit;
        private long _userId;
        private string _md5Key;

        public GameSession(IClusterClient client, ILoggerFactory loggerFactory,
            IConfiguration configuration, IChannelHandlerContext context)
        {
            _client = client;
            _logger = loggerFactory.CreateLogger<GameSession>();
            _configuration = configuration;
            _context = context;
            _md5Key = _configuration.GetValue<string>("MD5Key");
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
                var sign = packet.Sign;
                packet.Sign = string.Empty;
                var data = packet.ToByteString();
                if (CryptoHelper.MD5_Encrypt($"{data}{_md5Key}").ToLower() != sign.ToLower())
                {
                    await DispatchOutcomingPacket(packet.ParseResult(MOErrorType.Hidden, "签名验证失败"));
                    await Close();
                    return;
                }

                //同步初始化
                if (!_IsInit)
                {
                    _tokenGrain = _client.GetGrain<ITokenGrain>(packet.UserId);
                    var tokenInfo = _tokenGrain.GetToken().Result;
                    if (tokenInfo.Token != packet.Token || tokenInfo.LastTime.AddSeconds(GameConstants.TOKENEXPIRE) < DateTime.Now)
                    {
                        await DispatchOutcomingPacket(packet.ParseResult(MOErrorType.Hidden, "Token验证失败"));
                        await Close();
                        return;
                    }
                    _userId = packet.UserId;
                    _packetObserver = new OutcomingPacketObserver(this);
                    _router = _client.GetGrain<IPacketRouterGrain>(_userId);
                    _user = _client.GetGrain<IUserGrain>(_userId);
                    _packetObserverRef = _client.CreateObjectReference<IPacketObserver>(_packetObserver);
                    _user.BindPacketObserver(_packetObserverRef).Wait();
                    _IsInit = true;
                }
                else
                {
                    var tokenInfo = _tokenGrain.GetToken().Result;
                    if (tokenInfo.Token != packet.Token || tokenInfo.LastTime.AddSeconds(GameConstants.TOKENEXPIRE) < DateTime.Now)
                    {
                        await DispatchOutcomingPacket(packet.ParseResult(MOErrorType.Hidden, "Token验证失败"));
                        await Close();
                        return;
                    }
                }

                //心跳包
                if (packet.ActionId == 1)
                {
                    await _tokenGrain.RefreshTokenTime();
                    await DispatchOutcomingPacket(packet.ParseResult());
                    return;
                }

                await _router.SendPacket(packet);
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
            private readonly GameSession session;

            public OutcomingPacketObserver(GameSession session)
            {
                this.session = session;
            }

            public async Task Close(MOMsg packet = null)
            {
                if (packet != null)
                    await session.DispatchOutcomingPacket(packet);
                await session.Close();
            }

            public async Task SendPacket(MOMsg packet)
            {
                await session.DispatchOutcomingPacket(packet);
            }
        }
    }
}
