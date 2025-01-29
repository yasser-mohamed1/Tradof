using System.Diagnostics;

namespace Tradof.Api
{
    public class PerformanceMiddleware(ILogger<PerformanceMiddleware> _logger, RequestDelegate _next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            const int performanceTimeLog = 500;
            var sw = new Stopwatch();
            sw.Start();
            await _next(context);
            sw.Stop();
            if (sw.ElapsedMilliseconds > performanceTimeLog)
            {
                _logger.LogWarning("request {method} {path} took about {elapsed} ms",
                context.Request?.Method,
                context.Request.Path.Value,
                sw.ElapsedMilliseconds);
            }
        }
    }
}
