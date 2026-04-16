using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Domain.Entities.V1;
using Template.Security.Entities;
using Template.Security.Seeds;

namespace Template.Security.Configurations.V1
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions");

            builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.HasQueryFilter(rp => !rp.Role.IsDeleted && !rp.Permission.IsDeleted);

            builder.HasData(RolePermissionSeed.Data);
        }
    }
}