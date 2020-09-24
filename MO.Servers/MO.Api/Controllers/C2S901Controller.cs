using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Orleans;
using ProtoMessage;

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
