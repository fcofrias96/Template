using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.Contracts.Repositories.V1;
using Template.Persistence.DbContexts;
using Template.Persistence.DbContexts.V1;
using Template.Persistence.OptionActions;
using Template.Persistence.Repositories.V1;

namespace Template.Persistence
{
    public static class ServiceExtensions
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region DbContext

            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(DbContextAction.InMemoryOptions);
                services.AddDbContext<AuditDbContext>(DbContextAction.AuditInMemoryOptions);
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(DbContextAction.DbOptions(configuration));
                services.AddDbContext<AuditDbContext>(DbContextAction.AuditDbOptions(configuration));
            }

            #endregion DbContext

            #region Dependency Injection

            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBaseAsync<>));
            services.AddScoped<IItemRepositoryAsync, ItemRepositoryAsync>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            #endregion Dependency Injection
        }
    }
}