using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Template.BuildingBlocks.Infrastructure.Auditing;
using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.SharedKernel.Domain;
using Xunit;

namespace Template.BuildingBlocks.Tests
{
    /// <summary>
    /// The audit interceptor wired exactly like the application does it: through <c>AddModuleDbContext</c>.
    /// </summary>
    public class AuditSaveChangesInterceptorTests
    {
        [Fact]
        public async Task Insert_writes_a_single_audit_row_and_stamps_the_entity()
        {
            using var fixture = new Fixture();
            var product = new Product { Name = "Hammer", Price = 9.5m };

            fixture.Context.Products.Add(product);
            await fixture.Context.SaveChangesAsync();

            var audit = Assert.Single(fixture.AuditRows());
            Assert.Equal(AuditActions.Create.ToString(), audit.Action);
            Assert.Equal(nameof(Product), audit.TableName);
            Assert.Contains("Hammer", audit.NewValues);
            Assert.Null(audit.OldValues);
            Assert.NotEqual(default, product.CreatedAt);
        }

        [Fact]
        public async Task Update_writes_a_single_audit_row_with_only_the_changed_columns()
        {
            using var fixture = new Fixture();
            var product = new Product { Name = "Hammer", Price = 9.5m };
            fixture.Context.Products.Add(product);
            await fixture.Context.SaveChangesAsync();
            fixture.ClearAudit();

            product.Price = 12m;
            await fixture.Context.SaveChangesAsync();

            var audit = Assert.Single(fixture.AuditRows());
            Assert.Equal(AuditActions.Update.ToString(), audit.Action);
            Assert.NotNull(product.LastModifiedAt);

            var changedColumns = JsonConvert.DeserializeObject<List<string>>(audit.AffectedColumns!)!;
            Assert.Contains(nameof(Product.Price), changedColumns);
            Assert.DoesNotContain(nameof(Product.Name), changedColumns);
        }

        [Fact]
        public async Task Delete_writes_a_single_audit_row()
        {
            using var fixture = new Fixture();
            var product = new Product { Name = "Hammer", Price = 9.5m };
            fixture.Context.Products.Add(product);
            await fixture.Context.SaveChangesAsync();
            fixture.ClearAudit();

            fixture.Context.Products.Remove(product);
            fixture.Context.SaveChanges();

            var audit = Assert.Single(fixture.AuditRows());
            Assert.Equal(AuditActions.Delete.ToString(), audit.Action);
        }

        [Fact]
        public async Task Update_without_value_changes_writes_no_audit_row()
        {
            using var fixture = new Fixture();
            var product = new Product { Name = "Hammer", Price = 9.5m };
            fixture.Context.Products.Add(product);
            await fixture.Context.SaveChangesAsync();
            fixture.ClearAudit();

            fixture.Context.Entry(product).Property(p => p.Name).IsModified = true;
            await fixture.Context.SaveChangesAsync();

            Assert.Empty(fixture.AuditRows());
        }

        [Fact]
        public async Task Saving_several_entities_writes_one_row_per_entity()
        {
            using var fixture = new Fixture();

            fixture.Context.Products.AddRange(
                new Product { Name = "Hammer", Price = 9.5m },
                new Product { Name = "Saw", Price = 20m });
            await fixture.Context.SaveChangesAsync();

            Assert.Equal(2, fixture.AuditRows().Count);
        }

        [Fact]
        public async Task Masked_properties_are_recorded_as_changed_without_their_values()
        {
            using var fixture = new Fixture(options => options.Mask<Product>(nameof(Product.Secret)));
            var product = new Product { Name = "Hammer", Price = 9.5m, Secret = "top-secret" };
            fixture.Context.Products.Add(product);
            await fixture.Context.SaveChangesAsync();

            var created = Assert.Single(fixture.AuditRows());
            Assert.DoesNotContain("top-secret", created.NewValues);
            Assert.Contains(AuditOptions.MaskedValue, created.NewValues);

            fixture.ClearAudit();
            product.Secret = "new-secret";
            await fixture.Context.SaveChangesAsync();

            var updated = Assert.Single(fixture.AuditRows());
            Assert.Contains(nameof(Product.Secret), updated.AffectedColumns);
            Assert.DoesNotContain("secret", updated.OldValues + updated.NewValues);
        }

        [Fact]
        public void Module_contexts_get_every_registered_interceptor()
        {
            var probe = new ProbeInterceptor();
            using var fixture = new Fixture(registerExtra: services => services.AddSingleton<IInterceptor>(probe));

            fixture.Context.Products.Add(new Product { Name = "Hammer", Price = 1m });
            fixture.Context.SaveChanges();

            Assert.True(probe.Called);
            Assert.Single(fixture.AuditRows());
        }

        private sealed class Product : EntityBase
        {
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string? Secret { get; set; }
        }

        private sealed class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
            {
            }

            public DbSet<Product> Products => Set<Product>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Product>().HasKey(p => p.Id);
            }
        }

        private sealed class ProbeInterceptor : SaveChangesInterceptor
        {
            public bool Called { get; private set; }

            public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
            {
                Called = true;
                return result;
            }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly ServiceProvider _provider;
            private readonly IServiceScope _scope;

            public Fixture(Action<AuditOptions>? configureAudit = null, Action<IServiceCollection>? registerExtra = null)
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?> { ["UseInMemoryDatabase"] = "true" })
                    .Build();

                var services = new ServiceCollection();
                services.AddLogging();
                registerExtra?.Invoke(services);

                // Unique module name => isolated in-memory database per test.
                services.AddModuleDbContext<TestDbContext>(configuration, $"Test{Guid.NewGuid():N}", "test");

                if (configureAudit is not null)
                {
                    services.Configure(configureAudit);
                }

                _provider = services.BuildServiceProvider();
                _scope = _provider.CreateScope();
                Context = _scope.ServiceProvider.GetRequiredService<TestDbContext>();

                // The audit log is one shared in-memory database: start every test from an empty one.
                ClearAudit();
            }

            public TestDbContext Context { get; }

            public List<Audit> AuditRows()
            {
                using var audit = _provider.GetRequiredService<IDbContextFactory<AuditDbContext>>().CreateDbContext();
                return audit.AuditLogs.AsNoTracking().Where(a => a.TableName == nameof(Product)).ToList();
            }

            public void ClearAudit()
            {
                using var audit = _provider.GetRequiredService<IDbContextFactory<AuditDbContext>>().CreateDbContext();
                audit.AuditLogs.RemoveRange(audit.AuditLogs);
                audit.SaveChanges();
            }

            public void Dispose()
            {
                _scope.Dispose();
                _provider.Dispose();
            }
        }
    }
}
