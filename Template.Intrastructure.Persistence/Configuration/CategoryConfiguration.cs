using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Core.Domain.Entities;
using Template.Infrastructure.Persistence.Seeds;

namespace Template.Infrastructure.Persistence.Configuration
{
    public class CategoryConfiguration : AuditableBaseEntityConfiguration<Category, int>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            #region Table

            builder.ToTable("Category");

            #endregion Table

            #region Properties

            builder.Property(x => x.Name)
                   .HasColumnType("VARCHAR(15)")
                   .IsRequired();

            #endregion Properties

            #region Keys

            builder.HasKey(x => x.Id);

            #endregion Keys

            #region Data Seeding

            builder.HasData(CategorySeed.data);

            #endregion Data Seeding

            base.Configure(builder);
        }
    }
}