using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Template.Persistence.DbContexts;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.OptionActions
{
    public static class DbContextAction
    {
        public static readonly Func<IConfiguration, Action<DbContextOptionsBuilder>> DbOptions = configuration =>
        {
            Action<DbContextOptionsBuilder> Options = (options) =>
            {
                var isDevelopment = configuration.GetValue<string>("Environment") == "Development"
                    || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

                options.UseSqlServer(configuration.GetConnectionString("TemplateMainConnection"),
                            optionBuilder => optionBuilder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                            .EnableRetryOnFailure(
                                maxRetryCount: 3,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null)
                             );

                if (isDevelopment)
                {
                    options.LogTo(Console.WriteLine, LogLevel.Information);
                }
            };

            return Options;
        };

        public static readonly Func<IConfiguration, Action<DbContextOptionsBuilder>> AuditDbOptions = configuration =>
        {
            Action<DbContextOptionsBuilder> Options = (options) =>
            {
                var isDevelopment = configuration.GetValue<string>("Environment") == "Development"
                    || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

                options.UseSqlServer(configuration.GetConnectionString("TemplateAuditConnection"),
                            optionBuilder => optionBuilder.MigrationsAssembly(typeof(AuditDbContext).Assembly.FullName)
                .EnableRetryOnFailure(
                                maxRetryCount: 3,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null));

                if (isDevelopment)
                {
                    options.EnableSensitiveDataLogging();
                }
            };

            return Options;
        };

        public static readonly Action<DbContextOptionsBuilder> InMemoryOptions = (options) =>
        {
            options.UseInMemoryDatabase("Template");
        };

        public static readonly Action<DbContextOptionsBuilder> AuditInMemoryOptions = (options) =>
        {
            options.UseInMemoryDatabase("TemplateAudit");
        };
    }
}