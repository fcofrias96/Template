using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        private const string ServiceName = "Template.Api";
        private const string ServiceVersion = "1.0.0";
        private const string ApplicationName = "Template";
        private const string AppInsightsConnectionStringKey = "APPLICATIONINSIGHTS_CONNECTION_STRING";
        
        public static void AddMonitoringInfrastructure(
            this WebApplicationBuilder builder,
            IConfiguration configuration)
        {
            var connectionString = configuration[AppInsightsConnectionStringKey];
            
            ConfigureSerilog(builder, configuration);
            ConfigureOpenTelemetry(builder, connectionString);
        }

        private static void ConfigureSerilog(WebApplicationBuilder builder, IConfiguration configuration)
        {
            var env = builder.Environment;

            builder.Host.UseSerilog((context, loggerConfiguration) =>
            {
                ConfigureBaseSerilog(loggerConfiguration);

                if (env.IsDevelopment())
                {
                    ConfigureDevelopmentLogging(loggerConfiguration);
                }
                else
                {
                    ConfigureProductionLogging(loggerConfiguration, configuration);
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

        private static void ConfigureDevelopmentLogging(LoggerConfiguration loggerConfiguration)
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

        private static void ConfigureProductionLogging(
            LoggerConfiguration loggerConfiguration, 
            IConfiguration configuration)
        {
            var aiConnectionString = configuration[AppInsightsConnectionStringKey];

            if (string.IsNullOrWhiteSpace(aiConnectionString))
            {
                throw new InvalidOperationException(
                    "Application Insights connection string is required for non-development environments. " +
                    $"Please configure '{AppInsightsConnectionStringKey}' in your application settings.");
            }

            loggerConfiguration.WriteTo.ApplicationInsights(
                aiConnectionString,
                TelemetryConverter.Traces);
        }

        private static void ConfigureOpenTelemetry(WebApplicationBuilder builder, string? connectionString)
        {
            var env = builder.Environment;

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource =>
                {
                    resource.AddService(serviceName: ServiceName, serviceVersion: ServiceVersion);
                })
                .WithMetrics(metrics => ConfigureMetrics(metrics, env, connectionString))
                .WithTracing(tracing => ConfigureTracing(tracing, env, connectionString));
        }

        private static void ConfigureMetrics(
            MeterProviderBuilder metrics, 
            IHostEnvironment env, 
            string? connectionString)
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter(ObservabilityMetrics.MeterName);

            if (env.IsProduction())
            {
                ValidateConnectionString(connectionString, "metrics");
                
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
            IHostEnvironment env, 
            string? connectionString)
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

            if (env.IsProduction())
            {
                ValidateConnectionString(connectionString, "traces");
                
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

        private static void ValidateConnectionString(string? connectionString, string telemetryType)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"Application Insights connection string is required for {telemetryType} in production. " +
                    $"Configure '{AppInsightsConnectionStringKey}' in your settings.");
            }
        }
    }
}
