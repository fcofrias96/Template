using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Template.Persistence.DbContexts;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.OptionActions
{
    public static class DbContextAction
    {
        private const string DevelopmentEnvironmentName = "Development";

        public static readonly Func<IConfiguration, Action<DbContextOptionsBuilder>> DbOptions = configuration =>
        {
            Action<DbContextOptionsBuilder> Options = (options) =>
            {
                var isDevelopment = configuration.GetValue<string>("Environment") == DevelopmentEnvironmentName
                    || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == DevelopmentEnvironmentName;
                
                var connectionString = configuration.GetConnectionString("TemplateMainConnection") 
                ?? throw new InvalidOperationException("Connection string 'TemplateMainConnection' not found.");

                options.UseSqlServer(connectionString,
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
                var isDevelopment = configuration.GetValue<string>("Environment") == DevelopmentEnvironmentName
                    || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == DevelopmentEnvironmentName;

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