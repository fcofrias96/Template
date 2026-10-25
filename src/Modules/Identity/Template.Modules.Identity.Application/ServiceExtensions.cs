using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Template.BuildingBlocks.Application.Extensions;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Application.Services;
using Template.Modules.Identity.Contracts.Authorization;

namespace Template.Modules.Identity.Application
{
    public static class ServiceExtensions
    {
        /// <summary>Use cases of the Identity module and the services it exposes to other modules.</summary>
        public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
        {
            services.AddServicesImplementingBaseService(typeof(ServiceExtensions).Assembly)
                    .AddScoped<IAuthEmailService, AuthEmailService>()
                    .AddScoped<IAuthService, AuthService>()
                    .AddScoped<IMenuService, MenuService>()

                    // Public contract consumed by other modules ([Permission] attribute).
                    .AddScoped<IPermissionChecker, PermissionChecker>();

            services.AddValidatorsFromAssembly(typeof(ServiceExtensions).Assembly);

            return services;
        }
    }
}
