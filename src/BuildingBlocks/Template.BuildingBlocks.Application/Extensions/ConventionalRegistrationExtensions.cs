using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Template.BuildingBlocks.Application.Contracts.Services.Base;

namespace Template.BuildingBlocks.Application.Extensions
{
    public static class ConventionalRegistrationExtensions
    {
        /// <summary>Registers every <see cref="IBaseService{TEntity}"/> implementation of the given assemblies.</summary>
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
    }
}
