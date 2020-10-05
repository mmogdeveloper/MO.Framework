using Microsoft.Extensions.Logging;
using MO.Algorithm.Config;
using MO.Common;
using MO.GrainInterfaces;
using MO.GrainInterfaces.User;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.Grains.User
{
    public class UserIdFactoryGrain : Grain, IUserIdFactory
    {
        private readonly ILogger _logger;
        private readonly IPersistentState<Int64> _curUserId;
        public UserIdFactoryGrain(
            [PersistentState("CurUserId", StorageProviders.DefaultProviderName)] IPersistentState<Int64> curUserId,
            ILogger<UserGrain> logger)
        {
            _curUserId = curUserId;
            _logger = logger;
        }

        public override Task OnActivateAsync()
        {
            _curUserId.ReadStateAsync();
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            _curUserId.WriteStateAsync();
            return base.OnDeactivateAsync();
        }

        public async Task<Int64> GetNewUserId()
        {
            if (_curUserId.State == 0)
            {
                _curUserId.State = GameConstants.USERIDINIT;
            }
            _curUserId.State += RandomUtils.GetRandom(1, 1024);
            await _curUserId.WriteStateAsync();
            return _curUserId.State;
        }
    }
}
