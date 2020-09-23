using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using MO.Algorithm.Actions;
using MO.Algorithm.Actions.Enum;
using MO.Algorithm.Redis;
using MO.Common.Security;
using MO.GrainInterfaces;
using MO.GrainInterfaces.Game;
using MO.GrainInterfaces.Global;
using MO.GrainInterfaces.Network;
using MO.GrainInterfaces.User;
using MO.Model.Context;
using MO.Model.Entitys;
using Newtonsoft.Json;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using ProtoMessage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MO.Grains.Network
{
    /// <summary>
    /// Packet router grain. It send different packet to partial class by its session state.
    /// </summary>
    [Reentrant]
    internal partial class PacketRouterGrain : Grain, IPacketRouter
    {
        private MODataContext _dataContext;
        private MORecordContext _recordContext;
        //private IClientboundPacketSink _sink;
        private IPacketObserver _observer;
        private IGlobalWorld _globalWorld;
        private IRoomFactory _roomFactory;
        private IUser _user;
        public PacketRouterGrain(
            MODataContext dataContext,
            MORecordContext recordContext)
        {
            _dataContext = dataContext;
            _recordContext = recordContext;
        }

        public override Task OnActivateAsync()
        {
            //_sink = GrainFactory.GetGrain<IClientboundPacketSink>(this.GetPrimaryKey());
            _globalWorld = GrainFactory.GetGrain<IGlobalWorld>(0);
            _roomFactory = GrainFactory.GetGrain<IRoomFactory>(0);
            return base.OnActivateAsync();
        }

        public Task SetObserver(IPacketObserver observer)
        {
            _observer = observer;
            return Task.CompletedTask;
        }

        public async Task SendPacket(MOMsg packet)
        {
            if (packet.ActionId == 100000)
            {
                //登录绑定
                _user = GrainFactory.GetGrain<IUser>(packet.UserId);
                await _user.BindPacketObserver(_observer);
                await _globalWorld.PlayerEnterGlobalWorld(_user);
                _observer.SendPacket(packet.ParseResult());
            }
            else
            {
                if (_user == null)
                {
                    _observer.SendPacket(packet.ParseResult(ErrorType.Hidden, "用户未登录"));
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
        }

        public async Task Disconnect()
        {
            if (_user != null)
            {
                await _user.UnbindPacketObserver();

                var roomId = await _user.GetRoomId();
                if (roomId != 0)
                {
                    var room = GrainFactory.GetGrain<IRoom>(roomId);
                    //通知离线
                    var message = new MOMsg() { };
                    await room.RoomNotify(message);
                }
            }
        }
    }
}
