using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.BuildingBlocks.Web.Modules;
using Template.Modules.Catalog.Application;
using Template.Modules.Catalog.Infrastructure;

namespace Template.Modules.Catalog.Api
{
    /// <summary>
    /// Composition root of the Catalog module (categories and items): each layer registers itself.
    /// </summary>
    public static class CatalogModule
    {
        public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCatalogApplication();
            services.AddCatalogInfrastructure(configuration);
            services.AddModuleControllers(typeof(CatalogModule).Assembly);

            return services;
        }
    }
}
