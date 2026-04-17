using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Security.Entities;
using Template.Security.Seeds;

namespace Template.Security.Configurations.V1
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