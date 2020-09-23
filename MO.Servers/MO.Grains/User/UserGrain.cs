using Microsoft.Extensions.Logging;
using MO.GrainInterfaces;
using MO.GrainInterfaces.Network;
using MO.GrainInterfaces.User;
using Orleans;
using Orleans.Streams;
using ProtoMessage;
using System;
using System.Threading.Tasks;

namespace MO.Grains.User
{
    public class UserGrain : Grain, IUser, IAsyncObserver<MOMsg>
    {
        private IPacketObserver _observer;
        private StreamSubscriptionHandle<MOMsg> _globalHandler;
        private StreamSubscriptionHandle<MOMsg> _roomHandler;
        private int _roomId;
        private ILogger _logger;

        public UserGrain(ILogger<UserGrain> logger)
        {
            _logger = logger;
        }

        public override Task OnActivateAsync()
        {
            _logger.LogInformation($"{this.GetPrimaryKeyLong()} 加载数据");
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            _logger.LogInformation($"{this.GetPrimaryKeyLong()} 回写数据");
            return base.OnDeactivateAsync();
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
            if (_observer != null)
                _observer.Close();
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

        public async Task SubscribeGlobal(Guid streamId)
        {
            if (_globalHandler == null)
            {
                var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
                var stream = streamProvider.GetStream<MOMsg>(streamId, StreamProviders.Namespaces.ChunkSender);
                _globalHandler = await stream.SubscribeAsync(OnNextAsync);
            }
        }

        public Task UnsubscribeGlobal()
        {
            //取消全服消息订阅
            if (_globalHandler != null)
            {
                _globalHandler.UnsubscribeAsync().Wait();
                _globalHandler = null;
            }
            return Task.CompletedTask;
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

        public Task UnsubscribeRoom()
        {
            if (_roomHandler != null)
            {
                _roomHandler.UnsubscribeAsync().Wait();
                _roomHandler = null;
            }
            return Task.CompletedTask;
        }

        public Task SetRoomId(int roomId)
        {
            _roomId = roomId;
            return Task.CompletedTask;
        }

        public Task<int> GetRoomId()
        {
            return Task.FromResult(_roomId);
        }
    }
}
