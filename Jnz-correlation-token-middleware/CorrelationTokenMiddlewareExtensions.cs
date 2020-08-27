using Microsoft.AspNetCore.Builder;

namespace Jnz.CorrelationTokenMiddleware
{
    public static class CorrelationTokenMiddlewareExtensions
    {
        public static IApplicationBuilder AddCorrelationToken(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<CorrelationTokenMiddleware>();
        }

    }
}
