using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Template.BuildingBlocks.Application.Contracts.Repositories.Base;

namespace Template.BuildingBlocks.Infrastructure.Extensions
{
    public static class ConventionalRegistrationExtensions
    {
        /// <summary>Registers every <see cref="IBaseRepository{TEntity}"/> implementation of the given assemblies.</summary>
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
