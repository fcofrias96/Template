using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Template.BuildingBlocks.Infrastructure.Persistence;

namespace Template.Modules.Identity.Infrastructure
{
    public class IdentityModuleDbContextFactory : IDesignTimeDbContextFactory<IdentityModuleDbContext>
    {
        public IdentityModuleDbContext CreateDbContext(string[] args)
        {
            var configuration = DbContextOptionsExtensions.BuildDesignTimeConfiguration();

            var optionsBuilder = new DbContextOptionsBuilder<IdentityModuleDbContext>();
            optionsBuilder.UseModuleSqlServer<IdentityModuleDbContext>(
                configuration.GetModuleConnectionString(ServiceExtensions.ModuleName),
                IdentityModuleDbContext.Schema);

            return new IdentityModuleDbContext(optionsBuilder.Options);
        }
    }
}
