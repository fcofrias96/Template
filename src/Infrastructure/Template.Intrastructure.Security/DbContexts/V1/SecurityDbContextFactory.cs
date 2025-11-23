using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Template.Security.DbContexts.V1;

namespace Template.Persistence.DbContexts
{
    public class SecurityDbContextFactory : IDesignTimeDbContextFactory<SecurityDbContext>
    {
        public SecurityDbContext CreateDbContext(string[] args)
        {
                var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("SecurityMigrationConnection") 
                ?? throw new InvalidOperationException("Connection string 'SecurityMigrationConnection' not found.");

            var optionsBuilder = new DbContextOptionsBuilder<SecurityDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new SecurityDbContext(optionsBuilder.Options);
        }
    }
}