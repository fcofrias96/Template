using Microsoft.EntityFrameworkCore;
using Template.Domain.Entities.V1;

namespace Template.Persistence.DbContexts
{
    public class AuditDbContext : DbContext
    {
        public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
        {
        }

        public DbSet<Audit> AuditLogs { get; set; }
    }
}