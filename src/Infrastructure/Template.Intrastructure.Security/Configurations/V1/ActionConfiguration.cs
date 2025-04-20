using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Security.Seeds;
using ActionType = Template.Security.Entities.ActionType;

namespace Template.Security.Configurations.V1
{
    public class ActionConfiguration : EntityBaseConfiguration<ActionType>
    {
        public override void Configure(EntityTypeBuilder<ActionType> builder)
        {
            builder.Property(r => r.Name)
                .IsRequired();

            builder.HasMany(r => r.Permissions)
                .WithOne(r => r.ActionType)
                .HasForeignKey(r => r.ActionTypeId);

            builder.HasData(ActionSeed.Data);

            base.Configure(builder);
        }
    }
}