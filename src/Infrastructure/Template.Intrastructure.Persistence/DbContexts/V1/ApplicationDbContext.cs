using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Template.Application.Extensions;
using Template.Application.Models;
using Template.Domain.Contracts;
using Template.Domain.Entities.V1;
using Template.Persistence.Configuration.V1;
using Actions = Template.Application.Enums.Actions;
using Category = Template.Domain.Entities.V1.Category;

namespace Template.Persistence.DbContexts.V1
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _userName = "default";
        private readonly AuditDbContext _auditDbContext;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContext,
            AuditDbContext auditDbContext
            ) : base(options)
        {
            _userName = httpContext!.HttpContext!.GetUserName();
            _auditDbContext = auditDbContext;
        }

        public ApplicationDbContext()
        {
        }

        #region DbSets

        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }

        #endregion DbSets

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            LogEntityChanges();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            LogEntityChanges();
            return base.SaveChanges();
        }

        private void LogEntityChanges()
        {
            var auditList = new List<Audit>();

            foreach (var entry in ChangeTracker.Entries<IEntityBase>())
            {
                if (entry.State is EntityState.Detached or EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry();
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.UserName = _userName;

                #region SetAuditInfo

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _userName;
                        entry.Entity.Created = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _userName;
                        entry.Entity.LastModified = DateTime.UtcNow;
                        break;

                    default:
                        break;
                }

                #endregion SetAuditInfo

                #region AuditProperties

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
                            auditEntry.Action = Actions.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.Action = Actions.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified &&
                                property.OriginalValue?.ToString() != property.CurrentValue?.ToString())
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.Action = Actions.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }

                            break;
                    }

                    auditList.Add(auditEntry.ToAudit());
                }

                #endregion AuditProperties
            }

            _auditDbContext.AuditLogs.AddRange(auditList);
            _auditDbContext.SaveChanges();
        }
    }
}