using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base;

namespace Template.Application.Extensions
{
    public static class ConventionalRegistrationExtensions
    {
        public static IServiceCollection AddServicesImplementingBaseService(
    this IServiceCollection services,
    params Assembly[] assemblies)
        {
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IBaseService<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());

            return services;
        }

        public static IServiceCollection AddRepositoriesImplementingBaseRepository(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IBaseRepository<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());

            return services;
        }
    }

}
