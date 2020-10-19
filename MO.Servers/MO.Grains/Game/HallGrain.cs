using Microsoft.Extensions.Logging;
using MO.GrainInterfaces;
using MO.GrainInterfaces.Game;
using MO.GrainInterfaces.User;
using MO.Model.Context;
using MO.Protocol;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MO.Grains.Game
{
    public class HallGrain : Grain, IHallGrain
    {
        private readonly ILogger _logger;
        private readonly Dictionary<long, IUserGrain> _userDict;
        private readonly IAsyncStream<MOMsg> _stream;
        private readonly MODataContext _dataContext;

        public HallGrain(ILogger<HallGrain> logger, MODataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
            _userDict = new Dictionary<long, IUserGrain>();
            var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
            _stream = streamProvider.GetStream<MOMsg>(Guid.NewGuid(), StreamProviders.Namespaces.ChunkSender);
        }

        public override Task OnActivateAsync()
        {
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            return base.OnDeactivateAsync();
        }

        public Task<Guid> JoinHall(IUserGrain user)
        {
            _userDict[user.GetPrimaryKeyLong()] = user;
            return Task.FromResult(_stream.Guid);
        }

        public Task<Guid> LeaveHall(IUserGrain user)
        {
            _userDict.Remove(user.GetPrimaryKeyLong());
            return Task.FromResult(_stream.Guid);
        }

        public Task HallNotify(MOMsg msg)
        {
            return _stream.OnNextAsync(msg);
        }
    }
}
