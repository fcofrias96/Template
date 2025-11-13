using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

namespace Template.Application.SetupOptions
{
    public static class SeriLog
    {
        public static readonly Action<HostBuilderContext, LoggerConfiguration> Options =
            (hostBuilderContext, loggerConfiguration) =>
            {
                var env = hostBuilderContext.HostingEnvironment;

                loggerConfiguration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Serilog.AspNetCore.RequestLoggingMiddleware", LogEventLevel.Fatal)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", "Template")
                    .Enrich.WithThreadId()
                    .Enrich.WithMachineName()
                    // Always write to console (consumed by App Service / container)
                    .WriteTo.Console(new CompactJsonFormatter());

                if (env.IsDevelopment())
                {
                    // Development only: local file logging
                    loggerConfiguration
                        .WriteTo.Logger(options =>
                        {
                            options
                                .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
                                .WriteTo.File(
                                    path: "./Logs/Info/log-.json",
                                    rollingInterval: RollingInterval.Day,
                                    retainedFileCountLimit: 7,
                                    formatter: new JsonFormatter()
                                );
                        })
                        .WriteTo.Logger(options =>
                        {
                            options
                                .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                                .WriteTo.File(
                                    path: "./Logs/Errors/error-.json",
                                    rollingInterval: RollingInterval.Day,
                                    retainedFileCountLimit: 7,
                                    formatter: new JsonFormatter()
                                );
                        });
                }
                else
                {
                    // Production (or other environments): Cloud telemetry via Application Insights
                    var aiConnectionString = hostBuilderContext.Configuration["ApplicationInsights:ConnectionString"];

                    if (string.IsNullOrWhiteSpace(aiConnectionString))
                    {
                        throw new InvalidOperationException(
                            "Application Insights connection string is required for non-development environments. " +
                            "Please configure 'ApplicationInsights:ConnectionString' in your application settings.");
                    }

                    loggerConfiguration.WriteTo.ApplicationInsights(
                        aiConnectionString,
                        TelemetryConverter.Traces);
                }
            };
    }
}
