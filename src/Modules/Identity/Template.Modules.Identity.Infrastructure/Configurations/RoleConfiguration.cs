using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.Modules.Identity.Domain.Entities;
using Template.Modules.Identity.Infrastructure.Seeds;

namespace Template.Modules.Identity.Infrastructure.Configurations
{
    public class RoleConfiguration : EntityBaseConfiguration<ApplicationRole>
    {
        public override void Configure(EntityTypeBuilder<ApplicationRole> builder)
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
