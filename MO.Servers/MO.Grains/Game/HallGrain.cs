using Microsoft.Extensions.Logging;
using MO.GrainInterfaces;
using MO.GrainInterfaces.Game;
using MO.GrainInterfaces.User;
using MO.Model.Context;
using MO.Protocol;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MO.Grains.Game
{
    public class HallGrain : Grain, IHallGrain
    {
        private readonly ILogger _logger;
        private readonly Dictionary<long, IUserGrain> _userDict;
        private readonly MODataContext _dataContext;
        private IAsyncStream<MOMsg> _stream;
        private string _streamKey;

        public HallGrain(ILogger<HallGrain> logger, MODataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
            _userDict = new Dictionary<long, IUserGrain>();
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
            _streamKey = Guid.NewGuid().ToString("N");
            _stream = streamProvider.GetStream<MOMsg>(StreamId.Create(StreamProviders.Namespaces.ChunkSender, _streamKey));
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        public Task<string> JoinHall(IUserGrain user)
        {
            _userDict[user.GetPrimaryKeyLong()] = user;
            return Task.FromResult(_streamKey);
        }

        public Task<string> LeaveHall(IUserGrain user)
        {
            _userDict.Remove(user.GetPrimaryKeyLong());
            return Task.FromResult(_streamKey);
        }

        public Task HallNotify(MOMsg msg)
        {
            return _stream.OnNextAsync(msg);
        }
    }
}
