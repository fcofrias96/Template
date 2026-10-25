using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.BuildingBlocks.Infrastructure.Extensions;
using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.Modules.Catalog.Application.Contracts;

namespace Template.Modules.Catalog.Infrastructure
{
    public static class ServiceExtensions
    {
        public const string ModuleName = "Catalog";

        /// <summary>Persistence of the Catalog module: DbContext, repositories and unit of work.</summary>
        public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddModuleDbContext<CatalogDbContext>(configuration, ModuleName, CatalogDbContext.Schema);

            services.AddRepositoriesImplementingBaseRepository(typeof(ServiceExtensions).Assembly)
                    .AddScoped<ICatalogUnitOfWork, CatalogUnitOfWork>();

            return services;
        }
    }
}
