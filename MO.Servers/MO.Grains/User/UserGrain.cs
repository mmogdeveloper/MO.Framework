using Microsoft.Extensions.Logging;
using MO.Algorithm.Enum;
using MO.GrainInterfaces;
using MO.GrainInterfaces.Network;
using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace MO.Grains.User
{
    public class LocationState
    {
        public Int32 RoomId { get; set; }
    }

    public class UserInfoState
    {
        public string Nickname { get; set; }
        public string HeadIcon { get; set; }
    }

    public class UserGrain : Grain, IUser, IAsyncObserver<MOMsg>
    {
        private readonly IPersistentState<LocationState> _location;
        private readonly IPersistentState<UserInfoState> _userinfo;
        private readonly ILogger _logger;

        private IPacketObserver _observer;
        private StreamSubscriptionHandle<MOMsg> _globalHandler;
        private StreamSubscriptionHandle<MOMsg> _roomHandler;

        public UserGrain(
            [PersistentState("LocationState", StorageProviders.DefaultProviderName)] IPersistentState<LocationState> location,
            [PersistentState("UserInfoState", StorageProviders.DefaultProviderName)] IPersistentState<UserInfoState> userinfo,
            ILogger<UserGrain> logger)
        {
            _location = location;
            _userinfo = userinfo;
            _logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            await _location.ReadStateAsync();
            await _userinfo.ReadStateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await _location.WriteStateAsync();
            await _userinfo.WriteStateAsync();
            await base.OnDeactivateAsync();
        }

        #region 订阅消息

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.CompletedTask;
        }

        public async Task OnNextAsync(MOMsg item, StreamSequenceToken token = null)
        {
            await Notify(item);
        }

        #endregion

        public Task BindPacketObserver(IPacketObserver observer)
        {
            _observer = observer;
            return Task.CompletedTask;
        }

        public Task UnbindPacketObserver()
        {
            _observer = null;
            return Task.CompletedTask;
        }

        public Task Notify(MOMsg packet)
        {
            if (_observer != null)
                _observer.SendPacket(packet);
            return Task.CompletedTask;
        }

        public Task Kick()
        {
            if (_observer != null)
            {
                var packet = new MOMsg() { ErrorCode = (int)BaseErrorType.Shown, ErrorInfo = "您的账号异地登录" };
                _observer.Close(packet);
            }
            return Task.CompletedTask;
        }

        public async Task SubscribeGlobal(Guid streamId)
        {
            if (_globalHandler == null)
            {
                var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
                var stream = streamProvider.GetStream<MOMsg>(streamId, StreamProviders.Namespaces.ChunkSender);
                _globalHandler = await stream.SubscribeAsync(OnNextAsync);
            }
        }

        public async Task UnsubscribeGlobal()
        {
            //取消全服消息订阅
            if (_globalHandler != null)
            {
                await _globalHandler.UnsubscribeAsync();
                _globalHandler = null;
            }
        }

        public async Task SubscribeRoom(Guid streamId)
        {
            if (_roomHandler == null)
            {
                var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
                var stream = streamProvider.GetStream<MOMsg>(streamId, StreamProviders.Namespaces.ChunkSender);
                _roomHandler = await stream.SubscribeAsync(OnNextAsync);
            }
        }

        public async Task UnsubscribeRoom()
        {
            if (_roomHandler != null)
            {
                await _roomHandler.UnsubscribeAsync();
                _roomHandler = null;
            }
        }

        public Task SetRoomId(int roomId)
        {
            _location.State.RoomId = roomId;
            _location.WriteStateAsync();
            return Task.CompletedTask;
        }

        public Task<int> GetRoomId()
        {
            return Task.FromResult(_location.State.RoomId);
        }

        public Task SetUserName(string userName)
        {
            _userinfo.State.Nickname = userName;
            _userinfo.WriteStateAsync();
            return Task.CompletedTask;
        }

        public Task<string> GetUserName()
        {
            return Task.FromResult(_userinfo.State.Nickname);
        }
    }
}
