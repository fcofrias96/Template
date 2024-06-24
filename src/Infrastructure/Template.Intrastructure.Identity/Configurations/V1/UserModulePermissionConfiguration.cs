using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Identity.Entities;
using Template.Identity.Seeds;

namespace Template.Identity.Configurations.V1
{
    public class UserModulePermissionConfiguration : IEntityTypeConfiguration<UserModulePermission>
    {
        public void Configure(EntityTypeBuilder<UserModulePermission> builder)
        {
            builder.HasKey(p => new { p.UserId, p.ModuleId, p.PermissionId });

            builder.HasData(UserModulePermissionSeed.Data);
        }
    }
}