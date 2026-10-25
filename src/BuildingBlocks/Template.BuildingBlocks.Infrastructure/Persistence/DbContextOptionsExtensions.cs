using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Template.BuildingBlocks.Infrastructure.Persistence
{
    public static class DbContextOptionsExtensions
    {
        private const string DevelopmentEnvironmentName = "Development";

        /// <summary>
        /// SQL Server setup shared by every module: migrations live next to the module's DbContext and the
        /// migrations history table is kept in the module's own schema, so modules can share one database.
        /// </summary>
        public static DbContextOptionsBuilder UseModuleSqlServer<TContext>(
            this DbContextOptionsBuilder options,
            string connectionString,
            string schema)
            where TContext : DbContext
        {
            return options.UseSqlServer(connectionString, sqlServer => sqlServer
                .MigrationsAssembly(typeof(TContext).Assembly.FullName)
                .MigrationsHistoryTable("__EFMigrationsHistory", schema)
                .EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null));
        }

        public const string DefaultConnectionStringName = "Default";

        /// <summary>
        /// Connection string for a module: <c>ConnectionStrings:{moduleName}</c> when set (to give the module its own
        /// database), otherwise <c>ConnectionStrings:Default</c>. Modules use separate schemas, so sharing is safe.
        /// Used both at runtime and by the design-time factories.
        /// </summary>
        public static string GetModuleConnectionString(this IConfiguration configuration, string moduleName)
            => configuration.GetConnectionString(moduleName)
               ?? configuration.GetConnectionString(DefaultConnectionStringName)
               ?? throw new InvalidOperationException(
                   $"Connection string '{moduleName}' not found and no '{DefaultConnectionStringName}' connection string is configured.");

        public static bool IsDevelopment(this IConfiguration configuration)
            => configuration.GetValue<string>("Environment") == DevelopmentEnvironmentName
               || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == DevelopmentEnvironmentName;

        /// <summary>
        /// Builds configuration for design-time DbContext factories (dotnet ef), which run from the host folder.
        /// </summary>
        public static IConfiguration BuildDesignTimeConfiguration()
            => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
    }
}
