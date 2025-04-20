using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Security.Entities;
using Template.Security.Seeds;

namespace Template.Security.Configurations.V1
{
    public class PermissionConfiguration : EntityBaseConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasIndex(p => new { p.ResourceId, p.ActionTypeId })
                .IsUnique();

            builder.Property(p => p.ActionTypeId)
                .IsRequired();

            builder.HasMany(p => p.RolePermissions)
                .WithOne(p => p.Permission)
                .HasForeignKey(p => p.PermissionId);

            builder.HasData(PermissionSeed.Data);

            base.Configure(builder);
        }
    }
}