using Google.Protobuf;
using Microsoft.Extensions.Logging;
using MO.GrainInterfaces;
using MO.GrainInterfaces.Game;
using MO.GrainInterfaces.User;
using MO.Model.Context;
using Orleans;
using Orleans.Providers;
using Orleans.Streams;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.Grains.Game
{
    public class RoomGrain : Grain, IRoom
    {
        private readonly ILogger _logger;
        private readonly Dictionary<long, PlayerData> _playerDict;

        private IAsyncStream<MOMsg> _stream;
        private IDisposable _reminder;
        //private ProtoRoomInfo _roomInfo;

        public RoomGrain(ILogger<RoomGrain> logger)
        {
            _logger = logger;
            _playerDict = new Dictionary<long, PlayerData>();
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            //间隔1秒执行一次
            _reminder = RegisterTimer(
                OnTimerCallback,
                this,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));

            var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
            _stream = streamProvider.GetStream<MOMsg>(Guid.NewGuid(), StreamProviders.Namespaces.ChunkSender);

            //var roomFactory = GrainFactory.GetGrain<IRoomFactory>(this.GetPrimaryKeyLong());
            //_roomInfo = await roomFactory.GetRoomInfo((int)this.GetPrimaryKeyLong());
            //for (int i = 0; i < _roomInfo.RoomHeader.PlayerNum; i++)
            //{
            //    _seatDatas.Add(new SeatData(i));
            //}
        }

        public override async Task OnDeactivateAsync()
        {
            if (_reminder != null)
                _reminder.Dispose();

            await base.OnActivateAsync();
        }

        private async Task OnTimerCallback(object obj)
        {
            await RoomNotify(new MOMsg() { ActionId = 1 });
        }

        public async Task RoomNotify(MOMsg msg)
        {
            await _stream.OnNextAsync(msg);
        }

        public async Task PlayerEnterRoom(IUser user)
        {
            _playerDict[user.GetPrimaryKeyLong()] = new PlayerData(user);
            await user.SubscribeRoom(_stream.Guid);
            {
                S2C100001 content = new S2C100001();
                content.RoomId = (int)this.GetPrimaryKeyLong();
                foreach (var item in _playerDict)
                {
                    PlayerData player = null;
                    if (_playerDict.TryGetValue(item.Key, out player))
                    {
                        content.UserPoints.Add(new UserPoint() { UserId = item.Key, Point = player.GetPoint() });
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
                MOMsg msg = new MOMsg();
                msg.ActionId = 100002;
                msg.Content = content.ToByteString();
                await RoomNotify(msg);
            }
        }

        public async Task PlayerLeaveRoom(IUser user)
        {
            _playerDict.Remove(user.GetPrimaryKeyLong());
            await user.UnsubscribeRoom();

            S2C100006 content = new S2C100006();
            content.UserId = user.GetPrimaryKeyLong();
            content.RoomId = (int)this.GetPrimaryKeyLong();
            MOMsg msg = new MOMsg();
            msg.ActionId = 100006;
            msg.Content = content.ToByteString();
            await RoomNotify(msg);
        }

        public Task PlayerReady(IUser user)
        {
            return Task.CompletedTask;
        }

        public async Task PlayerGo(IUser user,Int32 x,Int32 y)
        {
            S2C100004 content = new S2C100004();
            content.UserId = user.GetPrimaryKeyLong();
            content.Point = new MOPoint() { X = x, Y = y };
            MOMsg msg = new MOMsg();
            msg.ActionId = 100004;
            msg.Content = content.ToByteString();
            await RoomNotify(msg);
            if (_playerDict.ContainsKey(user.GetPrimaryKeyLong()))
            {
                _playerDict[user.GetPrimaryKeyLong()].SetPoint(new MOPoint() { X = x, Y = y });
            }
        }
    }
}
