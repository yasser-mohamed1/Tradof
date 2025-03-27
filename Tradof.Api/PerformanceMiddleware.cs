using System.Diagnostics;

namespace Tradof.Api
{
    public class PerformanceMiddleware(ILogger<PerformanceMiddleware> logger, RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            const int performanceTimeThreshold = 500; // Log requests that take >500ms

            var stopwatch = Stopwatch.StartNew();
            await next(context);
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > performanceTimeThreshold)
            {
                logger.LogWarning("Request {Method} {Path} took {Elapsed} ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
