using Microsoft.Extensions.Logging;
using MO.GrainInterfaces;
using MO.GrainInterfaces.Global;
using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MO.Grains.Global
{
    /// <summary>
    /// 数据统计
    /// </summary>
    public class GlobalWorldGrain : Grain, IGlobalWorldGrain
    {
        private readonly ILogger _logger;
        private readonly Dictionary<long, IUserGrain> _userDict;
        private IAsyncStream<MOMsg> _stream;
        private string _streamKey;

        public GlobalWorldGrain(ILogger<GlobalWorldGrain> logger)
        {
            _logger = logger;
            _userDict = new Dictionary<long, IUserGrain>();
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
            _streamKey = Guid.NewGuid().ToString("N");
            _stream = streamProvider.GetStream<MOMsg>(StreamId.Create(StreamProviders.Namespaces.ChunkSender, _streamKey));
            return base.OnActivateAsync(cancellationToken);
        }

        public Task Notify(MOMsg msg)
        {
            return _stream.OnNextAsync(msg);
        }

        public Task<int> GetCount()
        {
            return Task.FromResult(_userDict.Count);
        }

        public async Task PlayerEnterGlobalWorld(IUserGrain user)
        {
            _userDict[user.GetPrimaryKeyLong()] = user;
            await user.SubscribeGlobal(_streamKey);
        }

        public async Task PlayerLeaveGlobalWorld(IUserGrain user)
        {
            _userDict.Remove(user.GetPrimaryKeyLong());
            await user.UnsubscribeGlobal();
        }
    }
}
