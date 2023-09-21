using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace FortisPokerCard.WebService.Middleware
{
    public class RequestLoggerMiddleware
    {
        private const string LogMessage = "Request/Response";
        private readonly RequestDelegate _next;

        public RequestLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<RequestLoggerMiddleware> logger)
        {
            context.Request.EnableBuffering();
            Exception innerException = null;

            context.Response.OnStarting(
                (state) =>
                {
                    context.Response.Headers.Add("X-Request-Id", Activity.Current?.RootId);

                    return Task.FromResult(0);
                }, null);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                innerException = ex;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            logger.LogInformation(LogMessage);

            if (innerException != null)
                throw innerException;
        }
    }
}
