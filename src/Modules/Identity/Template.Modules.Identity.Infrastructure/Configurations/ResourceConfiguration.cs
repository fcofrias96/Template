using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.Modules.Identity.Domain.Entities;
using Template.Modules.Identity.Infrastructure.Seeds;

namespace Template.Modules.Identity.Infrastructure.Configurations
{
    public class ResourceConfiguration : EntityBaseConfiguration<Resource>
    {
        public override void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.Property(r => r.Name)
                .IsRequired();

            builder.HasMany(r => r.Permissions)
                .WithOne(r => r.Resource)
                .HasForeignKey(r => r.ResourceId);

            builder.HasData(ResourceSeed.Data);

            base.Configure(builder);
        }
    }
}
