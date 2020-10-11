using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MO.GrainInterfaces.Config;
using MO.GrainInterfaces.User;
using MO.Protocol;
using Newtonsoft.Json;
using Orleans;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MO.Api.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public abstract class BaseController : Controller
    {
        const string dataqueryname = "data";
        const string tokenqueryname = "token";
        const string useridqueryname = "userid";
        protected string data = string.Empty;
        protected string token = string.Empty;
        protected long userId = 0;
        protected IClusterClient client;

        public BaseController(IClusterClient client)
        {
            this.client = client;
        }

        [HttpGet]
        public abstract Task<string> GetMessage();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (context.HttpContext.Request.Query.ContainsKey(dataqueryname))
            {
                data = context.HttpContext.Request.Query[dataqueryname].ToString();
            }

            if (context.HttpContext.Request.Query.ContainsKey(tokenqueryname))
            {
                token = context.HttpContext.Request.Query[tokenqueryname].ToString();
            }

            if (context.HttpContext.Request.Query.ContainsKey(useridqueryname))
            {
                var struserid = context.HttpContext.Request.Query[useridqueryname].ToString();
                long.TryParse(struserid, out userId);
            }

            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var tokenGrain = client.GetGrain<IToken>(userId);
            var tokenInfo = tokenGrain.GetToken().Result;
            if (tokenInfo.Token != token ||
                tokenInfo.IP != ip ||
                tokenInfo.LastTime.AddSeconds(GameConstants.TOKENEXPIRE) < DateTime.Now)
            {
                var result = new MOMsgResult();
                result.ErrorCode = 10001;
                result.ErrorInfo = "Token Error";
                context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(result), Encoding.UTF8);
                return;
            }
        }
    }
}
