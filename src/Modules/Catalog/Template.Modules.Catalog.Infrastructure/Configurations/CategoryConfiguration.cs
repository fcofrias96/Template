using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.Modules.Catalog.Domain.Entities;
using Template.Modules.Catalog.Infrastructure.Seeds;

namespace Template.Modules.Catalog.Infrastructure.Configurations
{
    public class CategoryConfiguration : EntityBaseConfiguration<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            #region Properties

            builder.Property(x => x.Name)
                   .HasColumnType("VARCHAR(25)")
                   .IsRequired();

            #endregion Properties

            #region Keys

            builder.HasKey(x => x.Id);

            #endregion Keys

            #region Data Seeding

            builder.HasData(CategorySeed.Data);

            #endregion Data Seeding

            base.Configure(builder);
        }
    }
}
