using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MO.Login.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : Controller
    {
        const string queryname = "data";
        protected string data = string.Empty;
        [HttpGet]
        public abstract Task<string> GetMessage();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (context.HttpContext.Request.Query.ContainsKey(queryname))
            {
                data = context.HttpContext.Request.Query[queryname].ToString();
            }
        }
    }
}