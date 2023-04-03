using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.Application.Contracts.Repositories;
using Template.Infrastructure.Persistence.DbContexts;
using Template.Infrastructure.Persistence.OptionActions;
using Template.Infrastructure.Persistence.Repositories;

namespace Template.Infrastructure.Persistence
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

            services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddScoped<IItemRepositoryAsync, ItemRepository>();
            services.AddScoped<ICategoryRepositoryAsync, CategoryRepository>();

            #endregion Dependency Injection
        }
    }
}