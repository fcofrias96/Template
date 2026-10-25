using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.BuildingBlocks.Web.Modules;
using Template.Modules.Identity.Application;
using Template.Modules.Identity.Infrastructure;

namespace Template.Modules.Identity.Api
{
    /// <summary>
    /// Composition root of the Identity module (users, roles, permissions, menu, authentication):
    /// each layer registers itself.
    /// </summary>
    public static class IdentityModule
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityApplication();
            services.AddIdentityInfrastructure(configuration);
            services.AddModuleControllers(typeof(IdentityModule).Assembly);

            return services;
        }
    }
}
