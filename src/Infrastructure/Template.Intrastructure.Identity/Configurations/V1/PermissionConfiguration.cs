using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Identity.Entities;
using Template.Identity.Seeds;

namespace Template.Identity.Configurations.V1
{
    public class PermissionConfiguration : EntityBaseConfiguration<Permission, int>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.Name)
                .IsRequired();

            builder.HasMany(p => p.UserModulePermissions)
                .WithOne(p => p.Permission)
                .HasForeignKey(p => p.PermissionId);

            builder.HasData(PermissionSeed.Data);
        }
    }
}