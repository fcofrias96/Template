using Microsoft.EntityFrameworkCore;
using Template.Modules.Catalog.Domain.Entities;
using Template.Modules.Catalog.Infrastructure.Configurations;

namespace Template.Modules.Catalog.Infrastructure
{
    public class CatalogDbContext : DbContext
    {
        public const string Schema = "catalog";

        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
        {
        }

        #region DbSets

        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }

        #endregion DbSets

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        }
    }
}
