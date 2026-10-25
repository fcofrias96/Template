using Template.Modules.Identity.Contracts.Authorization;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Infrastructure.Seeds
{
    public static class RoleSeed
    {
        public static List<ApplicationRole> Data { get; } = new List<ApplicationRole>
        {
            new ApplicationRole
            {
                Id = 1,
                Name = Roles.Admin,
                NormalizedName = Roles.Admin.ToUpper(),
                // Fixed: IdentityRole assigns a random stamp, which would change the model on every build.
                ConcurrencyStamp = "3a7e5c1d-2b4f-4e6a-8c9d-0e1f2a3b4c01"
            },
            new ApplicationRole
            {
                Id = 2,
                Name = Roles.User,
                NormalizedName = Roles.User.ToUpper(),
                ConcurrencyStamp = "3a7e5c1d-2b4f-4e6a-8c9d-0e1f2a3b4c02"
            }
        };
    }
}
