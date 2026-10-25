using Microsoft.EntityFrameworkCore;

namespace Template.BuildingBlocks.Infrastructure.Auditing
{
    public class AuditDbContext : DbContext
    {
        public const string Schema = "audit";

        public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
        {
        }

        public DbSet<Audit> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);
        }
    }
}
