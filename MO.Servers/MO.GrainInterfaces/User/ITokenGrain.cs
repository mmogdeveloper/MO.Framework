using Orleans;
using System;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.User
{
    [GenerateSerializer]
    public class TokenInfo
    {
        [Id(0)]
        public string Token { get; set; }
        [Id(1)]
        public string IP { get; set; }
        [Id(2)]
        public DateTime LastTime { get; set; }
    }

    public interface ITokenGrain : IGrainWithIntegerKey
    {
        Task SetToken(string token, string ip);
        Task<TokenInfo> GetToken();
        Task RefreshTokenTime();
    }
}
