using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using MO.Protocol;
using Orleans;
using System.Threading.Tasks;

namespace MO.Api.Controllers
{
    public class C2S901Controller : BaseController
    {
        private IConfiguration _configuration;
        public C2S901Controller(IClusterClient client, IConfiguration configuration)
            : base(client)
        {
            _configuration = configuration;
        }

        public override Task<string> GetMessage()
        {
            Contacts message = new Contacts();
            message.WeiXinCode = _configuration.GetValue<string>("WeiXinCode");
            message.QQGroup = _configuration.GetValue<string>("QQGroup");
            message.WeiXinGroup = _configuration.GetValue<string>("WeiXinGroup");
            return Task.FromResult(new MOMsgResult()
            {
                Content = message.ToByteString()
            }.ToByteString().ToBase64());
        }
    }
}
