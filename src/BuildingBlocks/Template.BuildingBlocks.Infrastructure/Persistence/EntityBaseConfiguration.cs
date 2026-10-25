using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.SharedKernel.Domain;

namespace Template.BuildingBlocks.Infrastructure.Persistence
{
    /// <summary>
    /// Configures the columns of every role the entity implements (<see cref="IAuditable"/>,
    /// <see cref="ISoftDeletable"/>, <see cref="IHasRowVersion"/>), so entities only get what they opt into.
    /// </summary>
    public abstract class EntityBaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class, IEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            if (typeof(IAuditable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property<string>(nameof(IAuditable.CreatedBy))
                       .HasColumnType("nvarchar(30)")
                       .HasDefaultValue(AuditDefaults.SystemUser);

                builder.Property<DateTimeOffset>(nameof(IAuditable.CreatedAt))
                       .HasDefaultValueSql("CURRENT_TIMESTAMP");

                builder.Property<string?>(nameof(IAuditable.LastModifiedBy))
                       .HasColumnType("nvarchar(30)");

                builder.Property<DateTimeOffset?>(nameof(IAuditable.LastModifiedAt));
            }

            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property<bool>(nameof(ISoftDeletable.IsDeleted))
                       .HasDefaultValue(false);

                builder.HasQueryFilter(x => !EF.Property<bool>(x, nameof(ISoftDeletable.IsDeleted)));
            }

            if (typeof(IHasRowVersion).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property<byte[]>(nameof(IHasRowVersion.RowVersion))
                       .IsRowVersion()
                       .IsConcurrencyToken();
            }
        }
    }
}
