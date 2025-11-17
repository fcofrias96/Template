using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Template.Monitoring.Utilities;

namespace Template.Monitoring.Middlewares
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        // Umbral (en ms) para considerar un request como lento (solo para LOGS)
        private const int SlowRequestThresholdMs = 1000; // 1 segundo

        public RequestTimingMiddleware(
            RequestDelegate next,
            ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Puedes usar el TraceIdentifier como correlación
            var correlationId = context.TraceIdentifier;
            CorrelationContext.CorrelationId = correlationId;

            // Exponer el CorrelationId en el response
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Correlation-Id"] = correlationId;
                return Task.CompletedTask;
            });

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
                var path = context.Request.Path.ToString();
                var method = context.Request.Method;
                var statusCode = context.Response.StatusCode;

                // 🎯 MÉTRICA: se registra SIEMPRE
                ObservabilityMetrics.RequestDurationMs.Record(
                    elapsedMs,
                    new TagList
                    {
                        { "http.method", method },
                        { "http.path", path }, // si tienes template de ruta, mejor
                        { "http.status_code", statusCode }
                    });

                // 🧹 LOG: solo si supera el umbral de "lento"
                if (elapsedMs > SlowRequestThresholdMs)
                {
                    using (_logger.BeginScope(new Dictionary<string, object?>
                    {
                        ["CorrelationId"] = correlationId,
                        ["HttpMethod"] = method,
                        ["Path"] = path
                    }))
                    {
                        _logger.LogWarning(
                            "Slow request detected. {Method} {Path} responded {StatusCode} in {ElapsedMs} ms",
                            method, path, statusCode, elapsedMs);
                    }
                }
            }
        }
    }
}