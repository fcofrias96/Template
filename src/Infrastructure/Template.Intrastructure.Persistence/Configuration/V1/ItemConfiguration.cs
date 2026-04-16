using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Domain.Entities.V1;

namespace Template.Persistence.Configuration.V1
{
    public class ItemConfiguration : EntityBaseConfiguration<Item>
    {
        public override void Configure(EntityTypeBuilder<Item> builder)
        {
            #region Properties

            builder.Property(x => x.Name)
                    .HasColumnType("VARCHAR(50)")
                    .IsRequired();

            builder.Property(x => x.Price)
                   .HasColumnType("DECIMAL(13,2)")
                   .IsRequired();

            builder.Property(x => x.Description)
                   .HasColumnType("VARCHAR(100)")
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