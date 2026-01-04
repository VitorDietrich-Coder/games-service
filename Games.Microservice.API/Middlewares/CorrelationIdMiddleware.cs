using Games.Microservice.shared;
using System.Diagnostics;

namespace Games.Microservice.API.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            CorrelationContext.Current = correlationId!;

            context.Response.Headers[HeaderName] = correlationId!;

            await _next(context);
        }
    }
}
