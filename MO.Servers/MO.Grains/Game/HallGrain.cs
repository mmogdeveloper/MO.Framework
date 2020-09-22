using Microsoft.Extensions.Logging;
using MO.GrainInterfaces;
using MO.GrainInterfaces.Game;
using MO.GrainInterfaces.User;
using MO.Model.Context;
using Orleans;
using Orleans.Streams;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO.Grains.Game
{
    public class HallGrain : Grain, IHall
    {
        private readonly ILogger _logger;
        private readonly Dictionary<long, IUser> _userDict;
        private readonly IAsyncStream<MOMsg> _stream;
        private readonly MODataContext _dataContext;

        public HallGrain(ILogger<HallGrain> logger, MODataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
            _userDict = new Dictionary<long, IUser>();
            var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
            _stream = streamProvider.GetStream<MOMsg>(Guid.NewGuid(), StreamProviders.Namespaces.ChunkSender);
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await base.OnDeactivateAsync();
        }

        public Task<Guid> JoinHall(IUser user)
        {
            _userDict[user.GetPrimaryKeyLong()] = user;
            return Task.FromResult(_stream.Guid);
        }

        public Task<Guid> LeaveHall(IUser user)
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
