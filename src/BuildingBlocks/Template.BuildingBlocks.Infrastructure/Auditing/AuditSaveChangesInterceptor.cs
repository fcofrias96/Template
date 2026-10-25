using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Template.BuildingBlocks.Application.Extensions;
using Template.SharedKernel.Domain;

namespace Template.BuildingBlocks.Infrastructure.Auditing
{
    /// <summary>
    /// Cross-cutting auditing for every module DbContext, whatever its base class (e.g. IdentityDbContext):
    /// stamps <see cref="IAuditable"/> entities and writes one row per changed entity to the shared audit log.
    /// Registered once by <see cref="AuditingServiceExtensions.AddAuditing"/> and attached to module contexts
    /// through <c>AddModuleDbContext</c>.
    /// </summary>
    public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;
        private readonly AuditOptions _options;

        // Resolved lazily: the audit context's own options also resolve the registered interceptors.
        private IDbContextFactory<AuditDbContext>? _auditContextFactory;

        public AuditSaveChangesInterceptor(
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            IOptions<AuditOptions> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        private IDbContextFactory<AuditDbContext> AuditContextFactory
            => _auditContextFactory ??= _serviceProvider.GetRequiredService<IDbContextFactory<AuditDbContext>>();

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var audits = CollectChanges(eventData.Context);

            if (audits.Count > 0)
            {
                using var auditContext = AuditContextFactory.CreateDbContext();
                auditContext.AuditLogs.AddRange(audits);
                auditContext.SaveChanges();
            }

            return result;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var audits = CollectChanges(eventData.Context);

            if (audits.Count > 0)
            {
                await using var auditContext = await AuditContextFactory.CreateDbContextAsync(cancellationToken);
                auditContext.AuditLogs.AddRange(audits);
                await auditContext.SaveChangesAsync(cancellationToken);
            }

            return result;
        }

        private List<Audit> CollectChanges(DbContext? context)
        {
            var audits = new List<Audit>();

            if (context is null or AuditDbContext)
            {
                return audits;
            }

            var userId = _httpContextAccessor.HttpContext.GetUserId();

            foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
            {
                if (entry.State is EntityState.Detached or EntityState.Unchanged)
                    continue;

                Stamp(entry, userId);

                var auditEntry = BuildAuditEntry(entry, userId);

                // One audit row per changed entity; skip updates that did not change any value.
                if (entry.State == EntityState.Modified && auditEntry.ChangedColumns.Count == 0)
                    continue;

                audits.Add(auditEntry.ToAudit());
            }

            return audits;
        }

        private static void Stamp(EntityEntry<IAuditable> entry, string userId)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.StampCreated(userId, DateTimeOffset.UtcNow);
                    break;

                case EntityState.Modified:
                    entry.Entity.StampModified(userId, DateTimeOffset.UtcNow);
                    break;
            }
        }

        private AuditEntry BuildAuditEntry(EntityEntry<IAuditable> entry, string userId)
        {
            var entityType = entry.Entity.GetType();
            var auditEntry = new AuditEntry
            {
                TableName = entityType.Name,
                UserName = userId
            };

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;

                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                var masked = _options.IsMasked(entityType, propertyName);
                object? Value(object? value) => masked ? AuditOptions.MaskedValue : value;

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.Action = AuditActions.Create;
                        auditEntry.NewValues[propertyName] = Value(property.CurrentValue);
                        break;

                    case EntityState.Deleted:
                        auditEntry.Action = AuditActions.Delete;
                        auditEntry.OldValues[propertyName] = Value(property.OriginalValue);
                        break;

                    case EntityState.Modified:
                        if (property.IsModified &&
                            property.OriginalValue?.ToString() != property.CurrentValue?.ToString())
                        {
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.Action = AuditActions.Update;
                            auditEntry.OldValues[propertyName] = Value(property.OriginalValue);
                            auditEntry.NewValues[propertyName] = Value(property.CurrentValue);
                        }

                        break;
                }
            }

            return auditEntry;
        }
    }
}
