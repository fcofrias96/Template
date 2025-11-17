using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Diagnostics;

namespace Template.Monitoring.Utilities
{
    public class DbTimingInterceptor : DbCommandInterceptor
    {
        private readonly ILogger<DbTimingInterceptor> _logger;

        // Umbral (en ms) para considerar un comando como lento (solo para LOGS)
        private const int SlowDbThresholdMs = 200;

        public DbTimingInterceptor(ILogger<DbTimingInterceptor> logger)
        {
            _logger = logger;
        }

        private static Stopwatch Start()
        {
            var sw = new Stopwatch();
            sw.Start();
            return sw;
        }

        // --- Ejemplo con ReaderExecutedAsync (lo mismo aplica a Scalar/NonQuery) ---

        public override async ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Start();

            var dbResult = await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);

            stopwatch.Stop();

            RecordMetricsAndMaybeLog(command, eventData, stopwatch.Elapsed.TotalMilliseconds);

            return dbResult;
        }

        public override async ValueTask<int> NonQueryExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Start();

            var dbResult = await base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);

            stopwatch.Stop();

            RecordMetricsAndMaybeLog(command, eventData, stopwatch.Elapsed.TotalMilliseconds);

            return dbResult;
        }

        public override async ValueTask<object?> ScalarExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            object? result,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Start();

            var dbResult = await base.ScalarExecutedAsync(command, eventData, result, cancellationToken);

            stopwatch.Stop();

            RecordMetricsAndMaybeLog(command, eventData, stopwatch.Elapsed.TotalMilliseconds);

            return dbResult;
        }

        private void RecordMetricsAndMaybeLog(DbCommand command, CommandExecutedEventData eventData, double elapsedMs)
        {
            var dbContextName = eventData.Context?.GetType().Name ?? "UnknownDbContext";
            var commandType = command.CommandType.ToString();

            // 🎯 MÉTRICA: se registra SIEMPRE
            ObservabilityMetrics.DbCommandDurationMs.Record(
                elapsedMs,
                new TagList
                {
                    { "db.system", "oracle" }, // o "sqlserver", "postgresql", etc.
                    { "db.context", dbContextName },
                    { "db.command_type", commandType }
                });

            // 🧹 LOG: solo si supera el umbral
            if (elapsedMs < SlowDbThresholdMs)
            {
                return;
            }

            var correlationId = CorrelationContext.CorrelationId ?? string.Empty;

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["DbContext"] = dbContextName
            }))
            {
                _logger.LogWarning(
                    "Slow DB command. Duration: {ElapsedMs} ms. CommandType: {CommandType}. CommandText: {CommandText}",
                    elapsedMs,
                    command.CommandType,
                    Truncate(command.CommandText, 1000));
            }
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }
    }
}