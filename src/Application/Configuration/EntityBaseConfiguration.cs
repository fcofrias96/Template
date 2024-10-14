using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Domain.Contracts;

namespace Template.Application.Configuration
{
    public abstract class EntityBaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class, IEntityBase
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(x => x.CreatedBy)
                   .HasColumnType("nvarchar(30)")
                   .HasDefaultValue("System");

            builder.Property(x => x.Created)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.LastModifiedBy)
                .HasColumnType("nvarchar(30)");

            builder.Property(x => x.LastModified);

            builder.Property(x => x.IsDeleted)
                   .HasDefaultValue(false);

            if(typeof(TEntity).Name != "Role")
            {
                builder.HasQueryFilter(x => !x.IsDeleted);
            }
            
        }
    }
}