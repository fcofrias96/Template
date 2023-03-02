using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Core.Domain.Entities;

namespace Template.Infrastructure.Persistence.Configuration
{
    public class ItemConfiguration : AuditableBaseEntityConfiguration<Item>
    {
        public override void Configure(EntityTypeBuilder<Item> builder)
        {
            #region Table

            builder.ToTable("Item");

            #endregion Table

            #region Properties

            builder.Property(x => x.Id)
                   .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.Name)
                    .HasColumnType("VARCHAR(50)")
                    .IsRequired();

  
            #endregion Properties

            #region Keys

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Category)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.CategoryId)
            .HasConstraintName("FK_Item_Category");


            #endregion Keys

            base.Configure(builder);
        }
    }
}