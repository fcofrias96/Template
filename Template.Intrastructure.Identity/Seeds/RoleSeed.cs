using Microsoft.AspNetCore.Identity;
using Template.Core.Domain.Constants;

namespace Template.Infrastructure.Identity.Seeds
{
    public static class RoleSeed
    {
        public static List<IdentityRole<string>> data = new List<IdentityRole<string>>
        {
            new IdentityRole
            {
                Name = Role.Admin,
                NormalizedName = Role.Admin.ToUpper()
            },
            new IdentityRole
            {
                Name = Role.Moderator,
                NormalizedName = Role.Moderator.ToUpper()
            },
            new IdentityRole
            {
                Name = Role.Basic,
                NormalizedName = Role.Basic.ToUpper()
            }
        };
    }
}