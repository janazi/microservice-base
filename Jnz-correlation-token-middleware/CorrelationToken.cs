using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace Jnz.CorrelationTokenMiddleware
{
    public class CorrelationTokenMiddleware
    {
        private const string CORRELATION_TOKEN_HEADER = "Correlation-token";
        private readonly RequestDelegate _next;

        public CorrelationTokenMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (!(!StringValues.IsNullOrEmpty(context.Request.Headers[CORRELATION_TOKEN_HEADER])
                && Guid.TryParse(context.Request.Headers[CORRELATION_TOKEN_HEADER], out Guid correlationToken)))
                correlationToken = Guid.NewGuid();

            context.Request.Headers.Add(CORRELATION_TOKEN_HEADER, correlationToken.ToString());
            await _next(context);
        }
    }
}
