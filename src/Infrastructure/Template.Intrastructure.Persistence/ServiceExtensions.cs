using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Repositories.V1;
using Template.Application.Repository;
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
                services.AddDbContext<DbContext, ApplicationDbContext>(DbContextAction.InMemoryOptions);
                services.AddDbContext<AuditDbContext>(DbContextAction.AuditInMemoryOptions);
            }
            else
            {
                services.AddDbContext<DbContext, ApplicationDbContext>(DbContextAction.DbOptions(configuration));
                services.AddDbContext<AuditDbContext>(DbContextAction.AuditDbOptions(configuration));
            }

            #endregion DbContext

            #region Dependency Injection

            // repositories
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBaseAsync<>));
            services.AddScoped(typeof(ICreateRepositoryAsync<>), typeof(CreateRepositoryAsync<>));
            services.AddScoped(typeof(IDeleteRepositoryAsync<>), typeof(DeleteRepositoryAsync<>));
            services.AddScoped(typeof(IReadRepositoryAsync<>), typeof(ReadRepositoryAsync<>));
            services.AddScoped(typeof(IUpdateRepositoryAsync<>), typeof(UpdateRepositoryAsync<>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IItemRepositoryAsync, ItemRepositoryAsync>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            #endregion Dependency Injection
        }
    }
}