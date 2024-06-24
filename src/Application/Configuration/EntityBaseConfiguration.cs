using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Domain.Common;

namespace Template.Application.Configuration
{
    public abstract class EntityBaseConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity> where TEntity : EntityBase<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(x => x.CreatedBy)
                  .HasDefaultValue("default");

            builder.Property(x => x.Created)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.LastModifiedBy);

            builder.Property(x => x.LastModified);

            builder.Property(x => x.Active)
                   .HasDefaultValue(true);

            builder.HasQueryFilter(x => x.Active);
        }
    }
}