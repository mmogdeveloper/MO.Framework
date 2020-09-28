using MO.Algorithm.Config;
using MO.GrainInterfaces.User;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.Grains.User
{
    [Reentrant]
    public class TokenGrain : Grain, IToken
    {
        private TokenInfo _tokenInfo;

        public TokenGrain()
        {
            _tokenInfo = new TokenInfo();
        }

        public Task SetToken(string token)
        {
            _tokenInfo.Token = token;
            _tokenInfo.LastTime = DateTime.Now;
            return Task.CompletedTask;
        }

        public Task<TokenInfo> GetToken()
        {
            return Task.FromResult(_tokenInfo);
        }

        public Task<bool> CheckToken(string token)
        {
            if (_tokenInfo.Token != token || _tokenInfo.LastTime.AddSeconds(GameConstants.TOKENEXPIRE) < DateTime.Now)
                return Task.FromResult(false);
            return Task.FromResult(true);
        }

        public Task RefreshTokenTime()
        {
            _tokenInfo.LastTime = DateTime.Now;
            return Task.CompletedTask;
        }
    }
}
