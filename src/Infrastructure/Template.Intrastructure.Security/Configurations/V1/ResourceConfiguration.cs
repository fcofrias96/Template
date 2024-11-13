using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Security.Entities;
using Template.Security.Seeds;

namespace Template.Security.Configurations.V1
{
    public class ResourceConfiguration : EntityBaseConfiguration<Resource>
    {
        public override void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.Property(r => r.Name)
                .IsRequired();

            builder.HasMany(r => r.ResourceActions)
                .WithOne(r => r.Resource)
                .HasForeignKey(r => r.ResourceId);

            builder.HasData(ResourceSeed.Data);

            base.Configure(builder);
        }
    }
}