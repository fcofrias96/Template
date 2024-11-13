using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Security.Entities;
using Template.Security.Seeds;

namespace Template.Security.Configurations.V1
{
    public class RoleConfiguration : EntityBaseConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId);

            builder.HasData(RoleSeed.Data);

            base.Configure(builder);
        }
    }
}