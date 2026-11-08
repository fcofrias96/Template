using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;

namespace Template.BuildingBlocks.Application.Utilities;

public static class HttpResilienceExtensions
{
    public const string RetryPipelineName = "template-retry";

    /// <summary>
    /// Agrega un pipeline de reintento exponencial (Microsoft.Extensions.Http.Resilience) con logs
    /// para errores transitorios de HTTP: HttpRequestException, timeouts, 408, 429 y 5xx.
    /// </summary>
    public static IHttpClientBuilder AddRetryResilienceHandler(this IHttpClientBuilder builder)
    {
        builder.AddResilienceHandler(RetryPipelineName, (pipeline, context) =>
        {
            var logger = context.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(typeof(HttpResilienceExtensions));

            pipeline.AddRetry(CreateRetryOptions(logger));
        });

        return builder;
    }

    /// <summary>
    /// Crea las opciones de reintento exponencial (2, 4, 8 segundos) con logs.
    /// </summary>
    public static HttpRetryStrategyOptions CreateRetryOptions(ILogger? logger = null)
    {
        return new HttpRetryStrategyOptions
        {
            // ShouldHandle por defecto: HttpRequestException, TimeoutRejectedException, 408, 429 y 5xx
            MaxRetryAttempts = 3,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromSeconds(2),
            UseJitter = false,
            OnRetry = args =>
            {
                if (logger is null)
                    return ValueTask.CompletedTask;

                var attempt = args.AttemptNumber + 1;

                if (args.Outcome.Exception is not null)
                {
                    logger.LogWarning(
                        args.Outcome.Exception,
                        "[Resilience Retry] Intento {RetryAttempt} fallido. Reintentando en {SleepTime}s...",
                        attempt,
                        args.RetryDelay.TotalSeconds);
                }
                else if (args.Outcome.Result is not null)
                {
                    logger.LogWarning(
                        "[Resilience Retry] Respuesta no exitosa (StatusCode: {StatusCode}) en intento {RetryAttempt}. Reintentando en {SleepTime}s...",
                        (int)args.Outcome.Result.StatusCode,
                        attempt,
                        args.RetryDelay.TotalSeconds);
                }

                return ValueTask.CompletedTask;
            }
        };
    }
}
