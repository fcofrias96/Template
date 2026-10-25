using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Template.BuildingBlocks.Infrastructure.Persistence;

namespace Template.BuildingBlocks.Infrastructure.Auditing
{
    public class AuditDbContextFactory : IDesignTimeDbContextFactory<AuditDbContext>
    {
        public AuditDbContext CreateDbContext(string[] args)
        {
            var configuration = DbContextOptionsExtensions.BuildDesignTimeConfiguration();

            var optionsBuilder = new DbContextOptionsBuilder<AuditDbContext>();
            optionsBuilder.UseModuleSqlServer<AuditDbContext>(
                configuration.GetModuleConnectionString(AuditingServiceExtensions.ConnectionStringName),
                AuditDbContext.Schema);

            return new AuditDbContext(optionsBuilder.Options);
        }
    }
}
