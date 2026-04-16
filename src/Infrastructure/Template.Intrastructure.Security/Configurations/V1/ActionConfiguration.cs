using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Security.Seeds;
using PermissionAction = Template.Domain.Entities.V1.PermissionAction;

namespace Template.Security.Configurations.V1
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