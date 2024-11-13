using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Security.Entities;
using Template.Security.Seeds;

namespace Template.Security.Configurations.V1
{
    public class ResourceActionConfiguration : EntityBaseConfiguration<ResourceAction>
    {
        public override void Configure(EntityTypeBuilder<ResourceAction> builder)
        {
            builder.HasIndex(p => new { p.ResourceId, p.ActionId })
                .IsUnique();

            builder.Property(p => p.ActionId)
                .IsRequired();

            builder.HasMany(p => p.RolePermissions)
                .WithOne(p => p.ResourceAction)
                .HasForeignKey(p => p.ResourceActionId);

            builder.HasData(ResourceActionSeed.Data);

            base.Configure(builder);
        }
    }
}