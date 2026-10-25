using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.BuildingBlocks.Infrastructure.Persistence;

namespace Template.BuildingBlocks.Infrastructure.Auditing
{
    public static class AuditingServiceExtensions
    {
        public const string ConnectionStringName = "Audit";

        /// <summary>
        /// Registers the shared audit log (<see cref="AuditDbContext"/>) and the <see cref="AuditSaveChangesInterceptor"/>
        /// that every module DbContext gets through <c>AddModuleDbContext</c>. Idempotent.
        /// </summary>
        public static IServiceCollection AddAuditing(this IServiceCollection services, IConfiguration configuration)
        {
            if (services.Any(descriptor => descriptor.ServiceType == typeof(AuditSaveChangesInterceptor)))
            {
                return services;
            }

            services.AddOptions<AuditOptions>();
            services.AddHttpContextAccessor();
            services.AddSingleton<AuditSaveChangesInterceptor>();
            services.AddSingleton<IInterceptor>(sp => sp.GetRequiredService<AuditSaveChangesInterceptor>());

            // A factory, so the interceptor can write the audit rows with a short-lived context of its own.
            services.AddDbContextFactory<AuditDbContext>(options =>
            {
                if (configuration.UseInMemoryDatabase())
                {
                    options.UseInMemoryDatabase("Template.Audit");
                    return;
                }

                options.UseModuleSqlServer<AuditDbContext>(
                    configuration.GetModuleConnectionString(ConnectionStringName),
                    AuditDbContext.Schema);

                if (configuration.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }
            });

            services.AddHealthChecks().AddDbContextCheck<AuditDbContext>("audit-db", tags: ["db"]);

            return services;
        }
    }
}
