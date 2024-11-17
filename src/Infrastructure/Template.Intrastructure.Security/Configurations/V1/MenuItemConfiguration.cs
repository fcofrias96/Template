using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Application.Configuration;
using Template.Security.Entities;
using Template.Security.Seeds;

namespace Template.Security.Configurations.V1
{
    public class MenuItemConfiguration : EntityBaseConfiguration<MenuItem>
    {
        public override void Configure(EntityTypeBuilder<MenuItem> builder)
        {

            builder.HasIndex(x => new { x.ParentId, x.Order })
                .IsUnique();

            builder.HasOne(e => e.Parent)
            .WithMany(e => e.Children)
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Url)
                .IsRequired(false)
                .HasMaxLength(200);

            builder.Property(e => e.Icon)
                .HasMaxLength(100);

            builder.HasOne(e => e.ResourceAction)
                .WithMany()
                .HasForeignKey(e => e.ResourceActionId);

            builder.HasData(MenuItemSeed.Data);

            base.Configure(builder);
        }
    }
}