using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Template.Infrastructure.Http;

public static class PollyPolicies
{
    /// <summary>
    /// Crea una política de reintento exponencial con logs para errores transitorios de HTTP.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger = null!)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // Maneja 5xx, 408, y HttpRequestException
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests) // Maneja 429 (Rate Limit)
            .WaitAndRetryAsync(
                retryCount: 3, // Número de reintentos
                sleepDurationProvider: retryAttempt =>
                {
                    // Retardo exponencial: 2, 4, 8 segundos
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                },
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Si tenemos logger, registramos detalles
                    if (logger != null)
                    {
                        if (outcome.Exception != null)
                        {
                            logger.LogWarning(
                                outcome.Exception,
                                "[Polly Retry] Intento {RetryAttempt} fallido. Reintentando en {SleepTime}s...",
                                retryAttempt,
                                timespan.TotalSeconds
                            );
                        }
                        else
                        {
                            logger.LogWarning(
                                "[Polly Retry] Respuesta no exitosa (StatusCode: {StatusCode}) en intento {RetryAttempt}. Reintentando en {SleepTime}s...",
                                (int)outcome.Result.StatusCode,
                                retryAttempt,
                                timespan.TotalSeconds
                            );
                        }
                    }
                });
    }
}
