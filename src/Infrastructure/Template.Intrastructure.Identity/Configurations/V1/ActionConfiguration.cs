using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Identity.Seeds;
using Action = Template.Identity.Entities.Action;

namespace Template.Identity.Configurations.V1
{
    public class ActionConfiguration : EntityBaseConfiguration<Action>
    {
        public override void Configure(EntityTypeBuilder<Action> builder)
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