using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Identity.Entities;
using Template.Identity.Seeds;

namespace Template.Identity.Configurations.V1
{
    public class ModuleConfiguration : EntityBaseConfiguration<Module, int>
    {
        public override void Configure(EntityTypeBuilder<Module> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.Name)
                .IsRequired();

            builder.HasMany(p => p.UserModulePermissions)
                .WithOne(p => p.Module)
                .HasForeignKey(p => p.ModuleId);

            builder.HasData(ModuleSeed.Data);
        }
    }
}