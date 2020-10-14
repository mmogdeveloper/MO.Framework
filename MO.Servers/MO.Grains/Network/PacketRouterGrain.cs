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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MO.Grains.Network
{
    /// <summary>
    /// Packet router grain. It send different packet to partial class by its session state.
    /// </summary>
    //[Reentrant]
    internal partial class PacketRouterGrain : Grain, IPacketRouter
    {
        private IPacketObserver _observer;
        private IGlobalWorld _globalWorld;
        private IRoomFactory _roomFactory;
        private IUser _user;
        private Stopwatch _watch;
        private ILogger _logger;

        public PacketRouterGrain(ILogger<PacketRouterGrain> logger)
        {
            _watch = new Stopwatch();
            _logger = logger;
        }

        public override Task OnActivateAsync()
        {
            _globalWorld = GrainFactory.GetGrain<IGlobalWorld>(0);
            _roomFactory = GrainFactory.GetGrain<IRoomFactory>(0);
            return base.OnActivateAsync();
        }

        public Task SetObserver(IPacketObserver observer)
        {
            _observer = observer;
            return Task.CompletedTask;
        }

        private void Notify(MOMsg packet)
        {
            if (_observer != null)
            {
                _observer.SendPacket(packet);
            }
        }

        public async Task SendPacket(MOMsg packet)
        {
            //_watch.Restart();
            if (packet.ActionId == 100000)
            {
                //登录绑定
                _user = GrainFactory.GetGrain<IUser>(packet.UserId);
                await _user.BindPacketObserver(_observer);
                await _globalWorld.PlayerEnterGlobalWorld(_user);
                var roomId = await _user.GetRoomId();
                if (roomId != 0)
                {
                    var curRoom = GrainFactory.GetGrain<IRoom>(roomId);
                    await curRoom.Reconnect(_user);
                    //通知上线
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
                    await curRoom.RoomNotify(message);
                }
                Notify(packet.ParseResult());
            }
            else
            {
                if (_user == null)
                {
                    Notify(packet.ParseResult(MOErrorType.Hidden, "用户未登录"));
                    return;
                }

                switch (packet.ActionId)
                {
                    case 100001:
                        {
                            var req = C2S100001.Parser.ParseFrom(packet.Content);
                            var roomId = await _user.GetRoomId();
                            if (roomId == 0)
                            {
                                roomId = req.RoomId;
                                await _user.SetRoomId(roomId);
                            }
                            await _user.SetRoomId(roomId);
                            var room = GrainFactory.GetGrain<IRoom>(roomId);
                            await room.PlayerEnterRoom(_user);
                        }
                        break;
                    case 100003:
                        {
                            var req = C2S100003.Parser.ParseFrom(packet.Content);
                            var roomId = await _user.GetRoomId();
                            var curRoom = GrainFactory.GetGrain<IRoom>(roomId);
                            await curRoom.PlayerGo(_user, req.Vector.X, req.Vector.Y, req.Vector.Z,
                                req.Rotation.X, req.Rotation.Y, req.Rotation.Z);
                        }
                        break;
                    case 100005:
                        {
                            var req = C2S100005.Parser.ParseFrom(packet.Content);
                            var roomId = await _user.GetRoomId();
                            var curRoom = GrainFactory.GetGrain<IRoom>(roomId);
                            await curRoom.PlayerLeaveRoom(_user);
                            await _user.SetRoomId(0);
                        }
                        break;
                    case 100007:
                        {
                            var req = C2S100007.Parser.ParseFrom(packet.Content);
                            var roomId = await _user.GetRoomId();
                            var curRoom = GrainFactory.GetGrain<IRoom>(roomId);
                            await curRoom.PlayerSendMsg(_user, req.Content);
                        }
                        break;
                    case 100009:
                        {
                            var req = C2S100009.Parser.ParseFrom(packet.Content);
                            var roomId = await _user.GetRoomId();
                            var curRoom = GrainFactory.GetGrain<IRoom>(roomId);
                            await curRoom.PlayerCommand(_user, req.Commands.ToList());
                        }
                        break;
                }
            }
            //_watch.Stop();
            //Console.WriteLine($"执行时间：{packet.UserId} {_watch.ElapsedMilliseconds} ms");
        }

        public async Task Disconnect()
        {
            if (_user != null)
            {
                await _user.UnbindPacketObserver();
                await _globalWorld.PlayerLeaveGlobalWorld(_user);

                var roomId = await _user.GetRoomId();
                if (roomId != 0)
                {
                    var curRoom = GrainFactory.GetGrain<IRoom>(roomId);
                    //通知离线
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
                    await curRoom.RoomNotify(message);
                }
            }
        }
    }
}
