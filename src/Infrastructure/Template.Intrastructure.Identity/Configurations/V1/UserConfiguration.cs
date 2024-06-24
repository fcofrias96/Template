using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Identity.Entities;
using Template.Identity.Seeds;

namespace Template.Identity.Configurations.V1
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasMany(p => p.UserModulePermissions)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            builder.Property(p => p.FirstName)
                .IsRequired();

            builder.Property(p => p.LastName)
                .IsRequired();

            builder.Property(p => p.Created)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.Active)
                .HasDefaultValue(true);

            builder.HasData(UserSeed.Data);
        }
    }
}