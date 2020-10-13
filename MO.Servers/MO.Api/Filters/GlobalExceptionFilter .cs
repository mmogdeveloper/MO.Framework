using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MO.Algorithm.Enum;
using MO.Protocol;
using Newtonsoft.Json;

namespace MO.Api.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private ILogger _logger;
        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            var result = new MOMsgResult() { ErrorCode = (int)MOErrorType.Hidden, ErrorInfo = "未知错误" };
            context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}
