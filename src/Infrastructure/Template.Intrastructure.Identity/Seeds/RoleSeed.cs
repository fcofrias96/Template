using Microsoft.AspNetCore.Identity;
using Template.Application.Constants;

namespace Template.Identity.Seeds
{
    public static class RoleSeed
    {
        public static List<IdentityRole<string>> Data { get; } = new List<IdentityRole<string>>
        {
            new IdentityRole
            {
                Name = Roles.Admin,
                NormalizedName = Roles.Admin.ToUpper()
            },
            new IdentityRole
            {
                Name = Roles.User,
                NormalizedName = Roles.User.ToUpper()
            }
        };
    }
}