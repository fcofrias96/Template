using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.BuildingBlocks.Infrastructure.Auditing;

namespace Template.BuildingBlocks.Infrastructure.Persistence
{
    public static class ModuleDbContextExtensions
    {
        /// <summary>
        /// Registers a module's DbContext the same way for every module:
        /// <list type="bullet">
        /// <item>SQL Server on the module's connection string and schema, or the in-memory provider (seeded at startup).</item>
        /// <item>Every registered <see cref="IInterceptor"/> (auditing, DB timing, ...), so cross-cutting concerns
        /// apply to all modules without them knowing about each other.</item>
        /// <item>A health check named after the module.</item>
        /// </list>
        /// </summary>
        public static IServiceCollection AddModuleDbContext<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string moduleName,
            string schema)
            where TContext : DbContext
        {
            services.AddAuditing(configuration);

            services.AddDbContext<TContext>((serviceProvider, options) =>
            {
                if (configuration.UseInMemoryDatabase())
                {
                    options.UseInMemoryDatabase($"Template.{moduleName}");
                }
                else
                {
                    options.UseModuleSqlServer<TContext>(configuration.GetModuleConnectionString(moduleName), schema);
                }

                options.AddInterceptors(serviceProvider.GetServices<IInterceptor>());

                if (configuration.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }
            });

            services.AddInMemoryDatabaseInitializer<TContext>(configuration);

            services.AddHealthChecks().AddDbContextCheck<TContext>($"{moduleName.ToLowerInvariant()}-db", tags: ["db"]);

            return services;
        }
    }
}
