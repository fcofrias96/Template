using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.Modules.Identity.Infrastructure.Seeds;
using PermissionAction = Template.Modules.Identity.Domain.Entities.PermissionAction;

namespace Template.Modules.Identity.Infrastructure.Configurations
{
    public class ActionConfiguration : EntityBaseConfiguration<PermissionAction>
    {
        public override void Configure(EntityTypeBuilder<PermissionAction> builder)
        {
            builder.Property(r => r.Name)
                .IsRequired();

            builder.HasMany(r => r.Permissions)
                .WithOne(r => r.Action)
                .HasForeignKey(r => r.ActionId);

            builder.HasData(ActionSeed.Data);

            base.Configure(builder);
        }
    }
}
