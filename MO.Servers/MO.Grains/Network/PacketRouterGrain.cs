using Google.Protobuf;
using Microsoft.Extensions.Logging;
using MO.Algorithm.Enum;
using MO.GrainInterfaces.Extensions;
using MO.GrainInterfaces.Game;
using MO.GrainInterfaces.Global;
using MO.GrainInterfaces.Network;
using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MO.Grains.Network
{
    internal partial class PacketRouterGrain : Grain, IPacketRouterGrain
    {
        private IGlobalWorldGrain _globalWorld;
        private IRoomGrain _curRoom;
        private IUserGrain _user;
        private ILogger _logger;

        public PacketRouterGrain(ILogger<PacketRouterGrain> logger)
        {
            _logger = logger;
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _globalWorld = GrainFactory.GetGrain<IGlobalWorldGrain>(0);
            return base.OnActivateAsync(cancellationToken);
        }

        public async Task SendPacket(MOMsg packet)
        {
            if (packet.ActionId == 100000)
            {
                _user = GrainFactory.GetGrain<IUserGrain>(packet.UserId);
                await _globalWorld.PlayerEnterGlobalWorld(_user);
                if (_curRoom != null)
                {
                    await _curRoom.Reconnect(_user);
                    var message = new MOMsg()
                    {
                        ActionId = 100,
                        Content =
                        new S2C100()
                        {
                            UserId = _user.GetPrimaryKeyLong(),
                            IsOnline = true
                        }.ToByteString()
                    };
                    await _curRoom.RoomNotify(message);
                }
                await _user.Notify(packet.ParseResult());
            }
            else
            {
                if (_user == null)
                {
                    await _user.Notify(packet.ParseResult(MOErrorType.Hidden, "用户未登录"));
                    return;
                }

                if (packet.ActionId == 100001)
                {
                    var req = C2S100001.Parser.ParseFrom(packet.Content);
                    if (_curRoom == null)
                    {
                        _curRoom = GrainFactory.GetGrain<IRoomGrain>(req.RoomId);
                    }
                    await _curRoom.PlayerEnterRoom(_user);
                }
                else
                {
                    if (_curRoom == null)
                    {
                        await _user.Notify(packet.ParseResult(MOErrorType.Hidden, "房间信息不存在"));
                        return;
                    }

                    switch (packet.ActionId)
                    {
                        case 100005:
                            {
                                await _curRoom.PlayerLeaveRoom(_user);
                                _curRoom = null;
                            }
                            break;
                        case 100007:
                            {
                                var req = C2S100007.Parser.ParseFrom(packet.Content);
                                await _curRoom.PlayerSendMsg(_user, req.Content);
                            }
                            break;
                        case 100009:
                            {
                                var req = C2S100009.Parser.ParseFrom(packet.Content);
                                await _curRoom.PlayerCommand(_user, req.Commands.ToList());
                            }
                            break;
                    }
                }
            }
        }

        public async Task Disconnect()
        {
            if (_user != null)
            {
                await _user.UnbindPacketObserver();
                await _user.UnsubscribeRoom();
                await _globalWorld.PlayerLeaveGlobalWorld(_user);
                if (_curRoom != null)
                {
                    var message = new MOMsg()
                    {
                        ActionId = 100,
                        Content =
                        new S2C100()
                        {
                            UserId = _user.GetPrimaryKeyLong(),
                            IsOnline = false
                        }.ToByteString()
                    };
                    await _curRoom.RoomNotify(message);
                }
            }
        }
    }
}
