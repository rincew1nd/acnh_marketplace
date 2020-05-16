// <copyright file="RequestLoggingMiddleware.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Web
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Middleware for logging all incoming request.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next"><see cref="RequestDelegate">Next request processer in pipeline</see>.</param>
        /// <param name="logger"><see cref="ILogger"/>.</param>
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        /// <summary>
        /// Invoke middleware.
        /// </summary>
        /// <param name="context"><see cref="HttpContext">Request context</see>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            finally
            {
                using var bodyReader = new StreamReader(context.Request.Body);
                string body = await bodyReader.ReadToEndAsync();
                this.logger.LogDebug(
                    "Request {method} {url} => {statusCode}. Body - {body}",
                    context.Request?.Method,
                    context.Request?.Path.Value,
                    context.Response?.StatusCode,
                    body);
            }
        }
    }
}