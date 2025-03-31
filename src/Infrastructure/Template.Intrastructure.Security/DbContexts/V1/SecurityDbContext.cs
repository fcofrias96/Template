using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Template.Security.Configurations.V1;
using Template.Security.Entities;

namespace Template.Security.DbContexts.V1
{
    public class SecurityDbContext : IdentityDbContext<User, Role, string>
    {
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options)
        {
        }

        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ResourceAction> Permissions { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        public new DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });

            builder.ApplyConfiguration(new ActionConfiguration());
            builder.ApplyConfiguration(new MenuItemConfiguration());
            builder.ApplyConfiguration(new ResourceActionConfiguration());
            builder.ApplyConfiguration(new ResourceConfiguration());
            builder.ApplyConfiguration(new RoleClaimConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new RolePermissionConfiguration());
            builder.ApplyConfiguration(new UserClaimConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());

            builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins"));
            builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens"));
            builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims"));
        }
    }
}