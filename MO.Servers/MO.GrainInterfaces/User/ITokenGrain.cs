using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.User
{
    public class TokenInfo
    {
        public string Token { get; set; }
        public string IP { get; set; }
        public DateTime LastTime { get; set; }
    }

    public interface ITokenGrain : IGrainWithIntegerKey
    {
        Task SetToken(string token, string ip);
        Task<TokenInfo> GetToken();
        Task RefreshTokenTime();
    }
}
