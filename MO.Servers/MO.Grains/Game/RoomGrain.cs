using Google.Protobuf;
using Microsoft.Extensions.Logging;
using MO.GrainInterfaces;
using MO.GrainInterfaces.Game;
using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MO.Grains.Game
{
    public class RoomInfo
    {
        public Int32 GameId { get; set; }
    }

    public class RoomGrain : Grain, IRoom
    {
        private readonly IPersistentState<RoomInfo> _roomInfo;
        private readonly Dictionary<long, PlayerData> _players;
        private readonly ILogger _logger;

        private IAsyncStream<MOMsg> _stream;
        private IDisposable _reminder;

        public RoomGrain(
            [PersistentState("RoomInfo", StorageProviders.DefaultProviderName)] IPersistentState<RoomInfo> roomInfo,
            ILogger<RoomGrain> logger)
        {
            _roomInfo = roomInfo;
            _logger = logger;
            _players = new Dictionary<long, PlayerData>();
        }

        public override async Task OnActivateAsync()
        {
            //自定义加载数据
            await _roomInfo.ReadStateAsync();
            
            //定时器
            _reminder = RegisterTimer(
                OnTimerCallback,
                this,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromMilliseconds(100));

            var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
            _stream = streamProvider.GetStream<MOMsg>(Guid.NewGuid(), StreamProviders.Namespaces.ChunkSender);

            //var roomFactory = GrainFactory.GetGrain<IRoomFactory>(this.GetPrimaryKeyLong());
            //_roomInfo = await roomFactory.GetRoomInfo((int)this.GetPrimaryKeyLong());
            //for (int i = 0; i < _roomInfo.RoomHeader.PlayerNum; i++)
            //{
            //    _seatDatas.Add(new SeatData(i));
            //}
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            if (_reminder != null)
                _reminder.Dispose();

            //回写数据
            await _roomInfo.WriteStateAsync();
            await base.OnDeactivateAsync();
        }

        private async Task OnTimerCallback(object obj)
        {
            //non-Reentrant 调用
            var grain = this.AsReference<IRoom>();
            await grain.Update();
        }

        public Task Update()
        {
            S2C100004 content = new S2C100004();
            foreach (var item in _players)
            {
                if (item.Value.IsMove)
                {
                    var userPoint = new UserPoint();
                    userPoint.UserId = item.Key;
                    userPoint.Vector = new MsgVector3();
                    userPoint.Vector.X = item.Value.X;
                    userPoint.Vector.Y = item.Value.Y;
                    userPoint.Vector.Z = item.Value.Z;
                    userPoint.Rotation = new MsgRotation();
                    userPoint.Rotation.X = item.Value.RX;
                    userPoint.Rotation.Y = item.Value.RY;
                    userPoint.Rotation.Z = item.Value.RZ;
                    content.UserPoints.Add(userPoint);
                    item.Value.IsMove = false;
                }
            }
            MOMsg msg = new MOMsg();
            msg.ActionId = 100004;
            msg.Content = content.ToByteString();
            return RoomNotify(msg);
        }

        public Task RoomNotify(MOMsg msg)
        {
            return _stream.OnNextAsync(msg);
        }

        public async Task Reconnect(IUser user)
        {
            await user.SubscribeRoom(_stream.Guid);
        }

        public async Task PlayerEnterRoom(IUser user)
        {
            if (!_players.ContainsKey(user.GetPrimaryKeyLong()))
                _players[user.GetPrimaryKeyLong()] = new PlayerData(user);

            await user.SubscribeRoom(_stream.Guid);

            {
                S2C100001 content = new S2C100001();
                content.RoomId = (int)this.GetPrimaryKeyLong();
                foreach (var item in _players)
                {
                    PlayerData player = null;
                    if (_players.TryGetValue(item.Key, out player))
                    {
                        var userPoint = new UserPoint();
                        userPoint.UserId = item.Key;
                        userPoint.UserName = await player.User.GetUserName();
                        userPoint.Vector = new MsgVector3();
                        userPoint.Vector.X = player.X;
                        userPoint.Vector.Y = player.Y;
                        userPoint.Vector.Z = player.Z;
                        userPoint.Rotation = new MsgRotation();
                        userPoint.Rotation.X = player.RX;
                        userPoint.Rotation.Y = player.RY;
                        userPoint.Rotation.Z = player.RZ;
                        content.UserPoints.Add(userPoint);
                    }
                }
                MOMsg msg = new MOMsg();
                msg.ActionId = 100001;
                msg.Content = content.ToByteString();
                await user.Notify(msg);
            }
            {
                S2C100002 content = new S2C100002();
                content.UserId = user.GetPrimaryKeyLong();
                content.RoomId = (int)this.GetPrimaryKeyLong();
                content.UserName = await user.GetUserName();
                MOMsg msg = new MOMsg();
                msg.ActionId = 100002;
                msg.Content = content.ToByteString();
                await RoomNotify(msg);
            }
        }

        public async Task PlayerLeaveRoom(IUser user)
        {
            S2C100006 content = new S2C100006();
            content.UserId = user.GetPrimaryKeyLong();
            content.RoomId = (int)this.GetPrimaryKeyLong();
            MOMsg msg = new MOMsg();
            msg.ActionId = 100006;
            msg.Content = content.ToByteString();
            await RoomNotify(msg);

            _players.Remove(user.GetPrimaryKeyLong());
            await user.UnsubscribeRoom();
        }

        public Task PlayerReady(IUser user)
        {
            return Task.CompletedTask;
        }

        public Task PlayerGo(IUser user, float x, float y, float z,
            float rx, float ry, float rz)
        {
            if (_players.ContainsKey(user.GetPrimaryKeyLong()))
            {
                _players[user.GetPrimaryKeyLong()].SetLocation(x, y, z, rx, ry, rz);
            }
            return Task.CompletedTask;
        }

        public async Task PlayerSendMsg(IUser user, string msg)
        {
            S2C100008 content = new S2C100008();
            content.UserId = user.GetPrimaryKeyLong();
            content.Content = msg;
            MOMsg notify = new MOMsg();
            notify.ActionId = 100008;
            notify.Content = content.ToByteString();
            await RoomNotify(notify);
        }

        public async Task PlayerCommand(IUser user, List<CommandInfo> commands)
        {
            S2C100010 content = new S2C100010();
            content.UserId = user.GetPrimaryKeyLong();
            content.Commands.AddRange(commands);
            MOMsg notify = new MOMsg();
            notify.ActionId = 100010;
            notify.Content = content.ToByteString();
            await RoomNotify(notify);
        }
    }
}
