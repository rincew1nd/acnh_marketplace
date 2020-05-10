    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ACNH_Marketplace.Web
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                using (var bodyReader = new StreamReader(context.Request.Body))
                {
                    string body = await bodyReader.ReadToEndAsync();
                    _logger.LogDebug(
                        "Request {method} {url} => {statusCode}. Body - {body}",
                        context.Request?.Method,
                        context.Request?.Path.Value,
                        context.Response?.StatusCode,
                        body);
                }
            }
        }
    }
}