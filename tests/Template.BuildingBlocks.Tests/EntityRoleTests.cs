using Microsoft.EntityFrameworkCore;
using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.SharedKernel.Domain;
using Xunit;

namespace Template.BuildingBlocks.Tests
{
    public class EntityRoleTests
    {
        [Fact]
        public async Task Soft_delete_marks_the_entity_deleted_and_hides_it_from_queries()
        {
            await using var context = CreateContext();
            var product = new Product { Name = "Hammer" };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            await new ProductRepository(context).SoftDeleteAsync(product, CancellationToken.None);

            Assert.True(product.IsDeleted);
            Assert.Empty(await context.Products.ToListAsync());
            Assert.Single(await context.Products.IgnoreQueryFilters().ToListAsync());
        }

        [Fact]
        public async Task Soft_delete_works_for_a_detached_entity()
        {
            var databaseName = Guid.NewGuid().ToString();
            int id;

            await using (var seed = CreateContext(databaseName))
            {
                var product = new Product { Name = "Hammer" };
                seed.Products.Add(product);
                await seed.SaveChangesAsync();
                id = product.Id;
            }

            await using var context = CreateContext(databaseName);
            var detached = await context.Products.AsNoTracking().SingleAsync(p => p.Id == id);

            await new ProductRepository(context).SoftDeleteAsync(detached, CancellationToken.None);

            Assert.True((await context.Products.IgnoreQueryFilters().AsNoTracking().SingleAsync(p => p.Id == id)).IsDeleted);
        }

        [Fact]
        public void Configuration_only_maps_the_roles_an_entity_implements()
        {
            using var context = CreateContext();

            var product = context.Model.FindEntityType(typeof(Product))!;
            Assert.NotNull(product.FindProperty(nameof(ISoftDeletable.IsDeleted)));
            Assert.NotNull(product.FindProperty(nameof(IAuditable.CreatedBy)));
            Assert.NotEmpty(product.GetDeclaredQueryFilters());

            var tag = context.Model.FindEntityType(typeof(Tag))!;
            Assert.Null(tag.FindProperty(nameof(ISoftDeletable.IsDeleted)));
            Assert.Null(tag.FindProperty(nameof(IAuditable.CreatedBy)));
            Assert.Empty(tag.GetDeclaredQueryFilters());
        }

        [Fact]
        public void Audit_stamps_can_only_be_set_through_IAuditable()
        {
            var product = new Product { Name = "Hammer" };
            var at = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

            Assert.Equal(AuditDefaults.SystemUser, product.CreatedBy);

            ((IAuditable)product).StampCreated("user-1", at);
            ((IAuditable)product).StampModified("user-2", at.AddDays(1));

            Assert.Equal("user-1", product.CreatedBy);
            Assert.Equal(at, product.CreatedAt);
            Assert.Equal("user-2", product.LastModifiedBy);
            Assert.Equal(at.AddDays(1), product.LastModifiedAt);
            Assert.Null(typeof(Product).GetProperty(nameof(IAuditable.CreatedBy))!.GetSetMethod());
        }

        private static TestDbContext CreateContext(string? databaseName = null)
            => new(new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options);

        private sealed class Product : EntityBase
        {
            public string Name { get; set; } = string.Empty;
        }

        /// <summary>Opts into identity only: no audit columns, no soft delete, no row version.</summary>
        private sealed class Tag : IEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private sealed class ProductConfiguration : EntityBaseConfiguration<Product>
        {
        }

        private sealed class TagConfiguration : EntityBaseConfiguration<Tag>
        {
        }

        private sealed class ProductRepository : BaseRepository<Product>
        {
            public ProductRepository(DbContext dbContext) : base(dbContext)
            {
            }
        }

        private sealed class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
            {
            }

            public DbSet<Product> Products => Set<Product>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new ProductConfiguration());
                modelBuilder.ApplyConfiguration(new TagConfiguration());
            }
        }
    }
}
