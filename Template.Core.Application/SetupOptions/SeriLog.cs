using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

namespace Template.Core.Application.SetupOptions
{
    public static class SeriLog
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Options = (hostBuilderContext, loggerContiguration) =>
        {
            loggerContiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Logger(options =>
            {
                options.Filter.ByIncludingOnly(filterOptions =>
                {
                    return filterOptions.Level == Serilog.Events.LogEventLevel.Information;
                })
                .WriteTo.File(
                    path: "./logs/log-.json",
                    rollingInterval: RollingInterval.Day,
                    formatter: new JsonFormatter()
                );

            })
             .WriteTo.Logger(options =>
             {
                 options.Filter.ByIncludingOnly(filterOptions =>
                 {
                     return filterOptions.Level == Serilog.Events.LogEventLevel.Error;
                 })
                .WriteTo.File(
                     path: "./logs/error-.json",
                     rollingInterval: RollingInterval.Day,
                     formatter: new JsonFormatter()
                 );

             })
            .WriteTo.Console(new CompactJsonFormatter())
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", "Template")
            .Enrich.WithClientIp()
            .Enrich.WithClientAgent()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadName()
            .Enrich.WithThreadId();

        };
    }
}