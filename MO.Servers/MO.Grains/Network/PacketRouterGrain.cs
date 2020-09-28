using Google.Protobuf;
using Microsoft.Extensions.Logging;
using MO.Algorithm;
using MO.Algorithm.Actions.Enum;
using MO.GrainInterfaces.Game;
using MO.GrainInterfaces.Global;
using MO.GrainInterfaces.Network;
using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System.Diagnostics;
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
                    Notify(packet.ParseResult(ErrorType.Hidden, "用户未登录"));
                    return;
                }

                switch (packet.ActionId)
                {
                    case 100001:
                        {
                            var req = C2S100001.Parser.ParseFrom(packet.Content);
                            await _user.SetRoomId(req.RoomId);
                            var room = GrainFactory.GetGrain<IRoom>(req.RoomId);
                            await room.PlayerEnterRoom(_user);
                        }
                        break;
                    case 100003:
                        {
                            var req = C2S100003.Parser.ParseFrom(packet.Content);
                            var roomId = await _user.GetRoomId();
                            var curRoom = GrainFactory.GetGrain<IRoom>(roomId);
                            await curRoom.PlayerGo(_user, req.X, req.Y);
                        }
                        break;
                    case 100005:
                        {
                            var req = C2S100005.Parser.ParseFrom(packet.Content);
                            var roomId = await _user.GetRoomId();
                            var curRoom = GrainFactory.GetGrain<IRoom>(roomId);
                            await curRoom.PlayerLeaveRoom(_user);
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
