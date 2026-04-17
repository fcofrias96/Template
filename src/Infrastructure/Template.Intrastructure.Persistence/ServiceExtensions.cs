using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.Contracts;
using Template.Application.Extensions;
using Template.Persistence.DbContexts;
using Template.Persistence.DbContexts.V1;
using Template.Persistence.OptionActions;
using Template.Persistence.Services.V1;

namespace Template.Persistence
{
    public static class ServiceExtensions
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region DbContext

            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(DbContextAction.InMemoryOptions)
                        .AddDbContext<AuditDbContext>(DbContextAction.AuditInMemoryOptions);
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(DbContextAction.DbOptions(configuration))
                        .AddDbContext<AuditDbContext>(DbContextAction.AuditDbOptions(configuration));
            }

            #endregion DbContext

            #region Dependency Injection



            //Repositories
            services.AddRepositoriesImplementingBaseRepository(typeof(ServiceExtensions).Assembly);

           
            //Services
            services.AddServicesImplementingBaseService(typeof(ServiceExtensions).Assembly)
                    .AddScoped<IUnitOfWork, UnitOfWork>();

            #endregion Dependency Injection
        }
    }
}