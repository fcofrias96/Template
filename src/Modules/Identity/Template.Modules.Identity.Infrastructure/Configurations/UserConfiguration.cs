using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.Modules.Identity.Domain.Entities;
using Template.Modules.Identity.Infrastructure.Seeds;

namespace Template.Modules.Identity.Infrastructure.Configurations
{
    public class UserConfiguration : EntityBaseConfiguration<ApplicationUser>
    {
        public override void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users");

            builder.HasIndex(p => p.PublicId).IsUnique();

            builder.Property(p => p.PublicId)
                .IsRequired()
                .HasDefaultValueSql("NEWID()");

            builder.Property(p => p.FirstName)
                .IsRequired();

            builder.Property(p => p.LastName)
                .IsRequired();

            builder.Property(p => p.RefreshToken)
                .IsRequired(false);

            builder.HasData(UserSeed.Data);

            base.Configure(builder);
        }
    }
}
