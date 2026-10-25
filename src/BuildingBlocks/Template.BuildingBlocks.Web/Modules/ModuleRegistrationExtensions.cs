using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Template.BuildingBlocks.Web.Modules
{
    public static class ModuleRegistrationExtensions
    {
        /// <summary>
        /// Makes the controllers of a module's Api assembly discoverable by MVC.
        /// Idempotent: an assembly already found through the host's dependency graph is not added twice.
        /// </summary>
        public static IServiceCollection AddModuleControllers(this IServiceCollection services, Assembly moduleAssembly)
        {
            services.AddControllers().ConfigureApplicationPartManager(manager =>
            {
                var alreadyRegistered = manager.ApplicationParts
                    .OfType<AssemblyPart>()
                    .Any(part => part.Assembly == moduleAssembly);

                if (!alreadyRegistered)
                {
                    manager.ApplicationParts.Add(new AssemblyPart(moduleAssembly));
                }
            });

            return services;
        }
    }
}
