using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Template.BuildingBlocks.Infrastructure.Persistence;

namespace Template.Modules.Catalog.Infrastructure
{
    public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
    {
        public CatalogDbContext CreateDbContext(string[] args)
        {
            var configuration = DbContextOptionsExtensions.BuildDesignTimeConfiguration();

            var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();
            optionsBuilder.UseModuleSqlServer<CatalogDbContext>(
                configuration.GetModuleConnectionString(ServiceExtensions.ModuleName),
                CatalogDbContext.Schema);

            return new CatalogDbContext(optionsBuilder.Options);
        }
    }
}
