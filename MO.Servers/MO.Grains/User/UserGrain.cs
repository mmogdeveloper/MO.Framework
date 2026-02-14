using Microsoft.Extensions.Logging;
using MO.Algorithm.Enum;
using MO.GrainInterfaces;
using MO.GrainInterfaces.Network;
using MO.GrainInterfaces.User;
using MO.Model.Context;
using MO.Model.Entitys;
using MO.Protocol;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MO.Grains.User
{
    public class UserGrain : Grain, IUserGrain, IAsyncObserver<MOMsg>
    {
        private MODataContext _dataContext;
        private readonly ILogger _logger;

        private GameUser _gameUser;
        private IPacketObserver _observer;
        private StreamSubscriptionHandle<MOMsg> _globalHandler;
        private StreamSubscriptionHandle<MOMsg> _roomHandler;

        public UserGrain(
            MODataContext dataContext,
            ILogger<UserGrain> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            _gameUser = _dataContext.GameUsers.First(m => m.UserId == this.GetPrimaryKeyLong());
        }

        public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            _dataContext.Update(_gameUser);
            await base.OnDeactivateAsync(reason, cancellationToken);
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

        public async Task Notify(MOMsg packet)
        {
            if (_observer != null)
                await _observer.SendPacket(packet);
        }

        public async Task Kick()
        {
            if (_observer != null)
            {
                var packet = new MOMsg() { ErrorCode = (int)MOErrorType.Shown, ErrorInfo = "您的账号异地登录" };
                await _observer.Close(packet);
            }
        }

        public async Task SubscribeGlobal(string streamId)
        {
            if (_globalHandler == null)
            {
                var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
                var stream = streamProvider.GetStream<MOMsg>(StreamId.Create(StreamProviders.Namespaces.ChunkSender, streamId));
                _globalHandler = await stream.SubscribeAsync(OnNextAsync);
            }
        }

        public async Task UnsubscribeGlobal()
        {
            if (_globalHandler != null)
            {
                await _globalHandler.UnsubscribeAsync();
                _globalHandler = null;
            }
        }

        public async Task SubscribeRoom(string streamId)
        {
            if (_roomHandler == null)
            {
                var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
                var stream = streamProvider.GetStream<MOMsg>(StreamId.Create(StreamProviders.Namespaces.ChunkSender, streamId));
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

        public Task SetNickName(string nickName)
        {
            _gameUser.NickName = nickName;
            _dataContext.Update(_gameUser);
            return Task.CompletedTask;
        }

        public Task<string> GetNickName()
        {
            return Task.FromResult(_gameUser.NickName);
        }
    }
}
