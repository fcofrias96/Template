using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Diagnostics;

namespace Template.BuildingBlocks.Monitoring.Utilities
{
    /// <summary>
    /// Records the duration of every DB command (metric) and logs the slow ones. Registered as an
    /// <see cref="IInterceptor"/>, so it is attached to every module DbContext by <c>AddModuleDbContext</c>.
    /// </summary>
    public class DbTimingInterceptor : DbCommandInterceptor
    {
        private readonly ILogger<DbTimingInterceptor> _logger;

        // Umbral (en ms) para considerar un comando como lento (solo para LOGS)
        private const int SlowDbThresholdMs = 200;

        public DbTimingInterceptor(ILogger<DbTimingInterceptor> logger)
        {
            _logger = logger;
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            Record(command, eventData);
            return base.ReaderExecuted(command, eventData, result);
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            Record(command, eventData);
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            Record(command, eventData);
            return base.NonQueryExecuted(command, eventData, result);
        }

        public override ValueTask<int> NonQueryExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            Record(command, eventData);
            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
        {
            Record(command, eventData);
            return base.ScalarExecuted(command, eventData, result);
        }

        public override ValueTask<object?> ScalarExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            object? result,
            CancellationToken cancellationToken = default)
        {
            Record(command, eventData);
            return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }

        private void Record(DbCommand command, CommandExecutedEventData eventData)
        {
            // EF measures the command itself; the "Executed" callbacks run after it has finished.
            var elapsedMs = eventData.Duration.TotalMilliseconds;
            var dbContextName = eventData.Context?.GetType().Name ?? "UnknownDbContext";
            var dbSystem = eventData.Context?.Database.ProviderName ?? "unknown";
            var commandType = command.CommandType.ToString();

            // 🎯 MÉTRICA: se registra SIEMPRE
            ObservabilityMetrics.DbCommandDurationMs.Record(
                elapsedMs,
                new TagList
                {
                    { "db.system", dbSystem },
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
