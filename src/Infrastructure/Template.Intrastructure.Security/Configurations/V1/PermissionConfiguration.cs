using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Domain.Entities.V1;
using Template.Security.Seeds;

namespace Template.Security.Configurations.V1
{
    public class PermissionConfiguration : EntityBaseConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasIndex(p => new { p.ResourceId, p.ActionId })
                .IsUnique();

            builder.Property(p => p.ActionId)
                .IsRequired();

            builder.HasMany(p => p.RolePermissions)
                .WithOne(p => p.Permission)
                .HasForeignKey(p => p.PermissionId);

            builder.HasData(PermissionSeed.Data);

            base.Configure(builder);
        }
    }
}