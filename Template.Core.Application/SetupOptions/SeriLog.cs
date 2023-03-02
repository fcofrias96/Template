using Microsoft.Extensions.Hosting;
using Serilog;

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
                    path: "./logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true
                );

            })
             .WriteTo.Logger(options =>
             {
                 options.Filter.ByIncludingOnly(filterOptions =>
                 {
                     return filterOptions.Level == Serilog.Events.LogEventLevel.Error;
                 })
                .WriteTo.File(
                     path: "./logs/error-.txt",
                     rollingInterval: RollingInterval.Day,
                     rollOnFileSizeLimit: true
                 );

             })
            .WriteTo.Console()
            .Enrich.FromLogContext();
        };
    }
}