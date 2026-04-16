using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Domain.Contracts;

namespace Template.Application.Configuration
{
    public abstract class EntityBaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class, IEntityBase
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.CreatedBy)
                   .HasColumnType("nvarchar(30)")
                   .HasDefaultValue("System");

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.LastModifiedBy)
                .HasColumnType("nvarchar(30)");

            builder.Property(x => x.LastModifiedAt);

            builder.Property(x => x.IsDeleted)
                   .HasDefaultValue(false);
            
            builder.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}