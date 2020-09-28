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
        public DateTime LastTime { get; set; }
    }

    public interface IToken : IGrainWithIntegerKey
    {
        Task SetToken(string token);
        Task<TokenInfo> GetToken();
        Task<bool> CheckToken(string token);
        Task RefreshTokenTime();
    }
}
