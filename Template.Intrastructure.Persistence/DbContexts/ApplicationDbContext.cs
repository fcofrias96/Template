using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Template.Core.Application.Extensions;
using Template.Core.Application.Models;
using Template.Core.Domain.Common;
using Template.Core.Domain.Entities;
using Template.Infrastructure.Persistence.Configuration;
using Action = Template.Core.Application.Emuns.Action;


namespace Template.Infrastructure.Persistence.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _userName = "default";

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContext) : base(options)
        {
            _userName = httpContext.HttpContext.GetUserName();
        }

        public ApplicationDbContext()
        {
        }

        #region DbSets

        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Audit> AuditLogs { get; set; }

        #endregion DbSets

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetEntry();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            SetEntry();
            return base.SaveChanges();
        }

        private void SetEntry()
        {
            var auditEntryList = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry();
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.UserName = _userName;
                auditEntryList.Add(auditEntry);

                #region AuditableBaseEntity

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _userName;
                        entry.Entity.Created = DateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _userName;
                        entry.Entity.LastModified = DateTime.Now;
                        break;

                    default:
                        break;
                }

                #endregion AuditableBaseEntity

                #region AuditLogs

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;

                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.Action = Action.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.Action = Action.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified &&
                                property.OriginalValue?.ToString() != property.CurrentValue?.ToString())
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.Action = Action.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }

                            break;
                    }
                }

                #endregion AuditLogs
            }

            foreach (var auditEntry in auditEntryList)
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }
        }
    }
}