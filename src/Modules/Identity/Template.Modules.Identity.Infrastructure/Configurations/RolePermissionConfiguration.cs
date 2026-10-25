using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Modules.Identity.Domain.Entities;
using Template.Modules.Identity.Infrastructure.Seeds;

namespace Template.Modules.Identity.Infrastructure.Configurations
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
