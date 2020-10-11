using MO.GrainInterfaces.User;
using Orleans;
using Orleans.Concurrency;
using System;
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

        public Task SetToken(string token, string ip)
        {
            _tokenInfo.Token = token;
            _tokenInfo.IP = ip;
            _tokenInfo.LastTime = DateTime.Now;
            return Task.CompletedTask;
        }

        public Task<TokenInfo> GetToken()
        {
            return Task.FromResult(_tokenInfo);
        }

        public Task RefreshTokenTime()
        {
            _tokenInfo.LastTime = DateTime.Now;
            return Task.CompletedTask;
        }
    }
}
