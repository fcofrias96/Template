using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Security.Entities;
using Template.Security.Seeds;

namespace Template.Security.Configurations.V1
{
    public class UserConfiguration : EntityBaseConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.Property(p => p.FirstName)
                .IsRequired();

            builder.Property(p => p.LastName)
                .IsRequired();

            builder.HasData(UserSeed.Data);

            base.Configure(builder);
        }
    }
}