using Microsoft.EntityFrameworkCore;
using Template.Core.Domain.Entities;

namespace Template.Infrastructure.Persistence.DbContexts
{
    public class AuditDbContext: DbContext
    {
        public AuditDbContext(DbContextOptions<AuditDbContext> options):base(options)
        {

        }
        public DbSet<Audit> AuditLogs { get; set; }
    }
}
