using Microsoft.Extensions.DependencyInjection;
using Template.BuildingBlocks.Application.Extensions;

namespace Template.Modules.Catalog.Application
{
    public static class ServiceExtensions
    {
        /// <summary>Use cases of the Catalog module.</summary>
        public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
        {
            services.AddServicesImplementingBaseService(typeof(ServiceExtensions).Assembly);

            return services;
        }
    }
}
