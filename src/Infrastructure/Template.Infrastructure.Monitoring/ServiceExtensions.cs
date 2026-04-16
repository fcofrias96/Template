
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using Template.Monitoring.Utilities;

namespace Template.Monitoring
{
    public static class ServiceExtensions
    {
        private const string ServiceName = "Template.Presentation.Api";
        private const string ServiceVersion = "1.0.0";
        private const string ApplicationName = "Appointment";
        private const string AppInsightsConnectionStringKey = "APPLICATIONINSIGHTS_CONNECTION_STRING";

        public static void AddMonitoringInfrastructure(
            this WebApplicationBuilder builder,
            IConfiguration configuration)
        {
            var connectionString = configuration[AppInsightsConnectionStringKey];

            ConfigureSerilog(builder, connectionString);
            ConfigureOpenTelemetry(builder, connectionString);
        }

        private static void ConfigureSerilog(WebApplicationBuilder builder, string? connectionString)
        {
            var isDeployed = !string.IsNullOrWhiteSpace(connectionString);

            builder.Host.UseSerilog((context, loggerConfiguration) =>
            {
                ConfigureBaseSerilog(loggerConfiguration);

                if (isDeployed)
                {
                    ConfigureDeployedLogging(loggerConfiguration, connectionString!);
                }
                else
                {
                    ConfigureLocalLogging(loggerConfiguration);
                }
            });
        }

        private static void ConfigureBaseSerilog(LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Serilog.AspNetCore.RequestLoggingMiddleware", LogEventLevel.Fatal)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", ApplicationName)
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .WriteTo.Console(new CompactJsonFormatter());
        }

        private static void ConfigureLocalLogging(LoggerConfiguration loggerConfiguration)
        {
            ConfigureFileLogging(loggerConfiguration, LogEventLevel.Information, "./Logs/Info/log-.json");
            ConfigureFileLogging(loggerConfiguration, LogEventLevel.Warning, "./Logs/Warnings/warning-.json");
            ConfigureFileLogging(loggerConfiguration, LogEventLevel.Error, "./Logs/Errors/error-.json");
        }

        private static void ConfigureFileLogging(
            LoggerConfiguration loggerConfiguration,
            LogEventLevel level,
            string path)
        {
            loggerConfiguration.WriteTo.Logger(options =>
            {
                options
                    .Filter.ByIncludingOnly(e => e.Level == level)
                    .WriteTo.File(
                        path: path,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        formatter: new JsonFormatter()
                    );
            });
        }

        private static void ConfigureDeployedLogging(
            LoggerConfiguration loggerConfiguration,
            string connectionString)
        {
            loggerConfiguration.WriteTo.ApplicationInsights(
                connectionString,
                TelemetryConverter.Traces);
        }

        private static void ConfigureOpenTelemetry(WebApplicationBuilder builder, string? connectionString)
        {
            var isDeployed = !string.IsNullOrWhiteSpace(connectionString);

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource =>
                {
                    resource.AddService(serviceName: ServiceName, serviceVersion: ServiceVersion);
                })
                .WithMetrics(metrics => ConfigureMetrics(metrics, isDeployed, connectionString))
                .WithTracing(tracing => ConfigureTracing(tracing, isDeployed, connectionString));
        }

        private static void ConfigureMetrics(
            MeterProviderBuilder metrics,
            bool isDeployed,
            string? connectionString)
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter(ObservabilityMetrics.MeterName);

            if (isDeployed)
            {
                metrics.AddAzureMonitorMetricExporter(options =>
                {
                    options.ConnectionString = connectionString;
                });
            }
            else
            {
                metrics.AddConsoleExporter();
            }
        }

        private static void ConfigureTracing(
            TracerProviderBuilder tracing,
            bool isDeployed,
            string? connectionString)
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

            if (isDeployed)
            {
                tracing.AddAzureMonitorTraceExporter(options =>
                {
                    options.ConnectionString = connectionString;
                });
            }
            else
            {
                tracing.AddConsoleExporter();
            }
        }
    }
}