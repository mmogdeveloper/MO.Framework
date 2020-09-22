using Google.Protobuf;
using Microsoft.Extensions.Logging;
using MO.Algorithm.Actions.Enum;
using MO.Algorithm.Redis;
using MO.Common;
using MO.Common.Config;
using MO.Common.Security;
using MO.Model.Context;
using MO.Model.Entitys;
using ProtoMessage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MO.Login.Controllers
{
    /// <summary>
    /// 游客登录
    /// </summary>
    public class C2S1003Controller : BaseController
    {
        private readonly ILogger _logger;
        private readonly MODataContext _dataContext;
        private readonly MORecordContext _recordContext;
        public C2S1003Controller(
            MODataContext dataContext,
            MORecordContext recordContext,
            ILogger<C2S1003Controller> logger)
        {
            _dataContext = dataContext;
            _recordContext = recordContext;
            _logger = logger;
        }

        public override Task<string> GetMessage()
        {
            var req1003 = C2S_1003.Parser.ParseFrom(ByteString.FromBase64(data));

            var user = _dataContext.GameUsers.Where(m => m.DeviceId == req1003.DeviceId).FirstOrDefault();
            long userId = 0;
            if (user == null || user.UserId == 0)
            {
                //注册账号
                if (TokenRedis.Client.Get<int>(RedisConstants.SKeyRedis_UserId) == 0)
                    TokenRedis.Client.Set(RedisConstants.SKeyRedis_UserId, GameConstants.USERIDINIT);
                userId = (long)TokenRedis.Client.IncrByFloat(RedisConstants.SKeyRedis_UserId, RandomUtils.GetRandom(1, 1024));
                user = new GameUser();
                user.UserId = userId;
                user.NickName = "游客001";
                user.HeadIcon = "1";
                user.DeviceId = req1003.DeviceId;
                _dataContext.Add(user);
                _dataContext.SaveChanges();
            }
            else
            {
                userId = user.UserId;
            }

            var token = CryptoHelper.MD5_Encrypt($"{userId}{DateTime.UtcNow.Ticks}");
            TokenRedis.Client.Set(userId.ToString(), token, GameConstants.TOKENEXPIRE);

            _recordContext.Add(new LoginRecord()
            {
                UserId = userId,
                LoginType = LoginType.None,
                LoginIP = HttpContext.Request.Host.Host,
                LoginDevice = req1003.DeviceId
            });

            _recordContext.SaveChanges();

            var serverconfig = _dataContext.ServerConfigs.Where(m => m.ServerLevel == 0).FirstOrDefault();
            if (serverconfig == null)
            {
                return Task.FromResult(new MOMsgResult() { ErrorCode = (int)ErrorType.Shown, ErrorInfo = "服务器不存在" }.ToByteString().ToBase64());
            }

            var message = new S2C_1003();
            message.Token = token;
            message.UserId = userId;
            message.LoginIP = serverconfig.LoginIP;
            message.LoginPort = serverconfig.LoginPort;
            message.ApiIP = serverconfig.ApiIP;
            message.ApiPort = serverconfig.ApiPort;
            message.GateIP = serverconfig.GateIP;
            message.GatePort = serverconfig.GatePort;

            return Task.FromResult(new MOMsgResult() { Content = message.ToByteString() }.ToByteString().ToBase64());
        }
    }
}
