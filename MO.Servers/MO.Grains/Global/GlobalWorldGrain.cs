using Microsoft.Extensions.Logging;
using MO.Common.Config;
using MO.GrainInterfaces;
using MO.GrainInterfaces.Global;
using MO.GrainInterfaces.User;
using Orleans;
using Orleans.Streams;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MO.Grains.Global
{
    /// <summary>
    /// 数据统计
    /// </summary>
    public class GlobalWorldGrain : Grain, IGlobalWorld
    {
        private readonly ILogger _logger;
        private readonly Dictionary<long, IUser> _userDict;
        private IAsyncStream<MOMsg> _stream;

        public GlobalWorldGrain(ILogger<GlobalWorldGrain> logger)
        {
            _logger = logger;
            _userDict = new Dictionary<long, IUser>();
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
            _stream = streamProvider.GetStream<MOMsg>(Guid.NewGuid(), StreamProviders.Namespaces.ChunkSender);
        }

        public Task Notify(MOMsg msg)
        {
            return _stream.OnNextAsync(msg);
        }

        public Task<int> GetCount()
        {
            return Task.FromResult(_userDict.Count);
        }

        public async Task PlayerEnterGlobalWorld(IUser user)
        {
            _userDict[user.GetPrimaryKeyLong()] = user;
            await user.SubscribeGlobal(_stream.Guid);
        }

        public async Task PlayerLeaveGlobalWorld(IUser user)
        {
            _userDict.Remove(user.GetPrimaryKeyLong());
            await user.UnsubscribeGlobal();
        }
    }
}
