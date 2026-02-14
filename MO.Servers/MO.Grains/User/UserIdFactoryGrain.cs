using Microsoft.Extensions.Logging;
using MO.GrainInterfaces.Config;
using MO.Common;
using MO.GrainInterfaces;
using MO.GrainInterfaces.User;
using Orleans;
using Orleans.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MO.Grains.User
{
    public class UserIdFactoryGrain : Grain, IUserIdFactoryGrain
    {
        private readonly ILogger _logger;
        private readonly IPersistentState<Int64> _curUserId;
        public UserIdFactoryGrain(
            [PersistentState("CurUserId", StorageProviders.DefaultProviderName)] IPersistentState<Int64> curUserId,
            ILogger<UserIdFactoryGrain> logger)
        {
            _curUserId = curUserId;
            _logger = logger;
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _curUserId.ReadStateAsync();
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            _curUserId.WriteStateAsync();
            return base.OnDeactivateAsync(reason, cancellationToken);
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
