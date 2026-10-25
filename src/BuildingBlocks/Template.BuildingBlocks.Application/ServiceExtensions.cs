using Microsoft.Extensions.DependencyInjection;
using Template.BuildingBlocks.Application.Caching;
using Template.BuildingBlocks.Application.Contracts.Services.Base.Export;
using Template.BuildingBlocks.Application.Services.Base;

namespace Template.BuildingBlocks.Application
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Services every module's base services depend on (caching, export).
        /// </summary>
        public static IServiceCollection AddApplicationBuildingBlocks(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            // Singleton: it owns the cache regions shared by every request.
            services.AddSingleton<ICacheService, MemoryCacheService>();

            services.AddScoped(typeof(IExportService<>), typeof(ExportService<>));

            return services;
        }
    }
}
