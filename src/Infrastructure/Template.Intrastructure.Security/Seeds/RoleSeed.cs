using Template.Application.Constants;
using Template.Domain.Entities.V1;
using Template.Security.Entities;

namespace Template.Security.Seeds
{
    public static class RoleSeed
    {
        public static List<ApplicationRole> Data { get; } = new List<ApplicationRole>
        {
            new ApplicationRole
            {
                Id = 1,
                Name = Roles.Admin,
                NormalizedName = Roles.Admin.ToUpper()
            },
            new ApplicationRole
            {
                Id = 2,
                Name = Roles.User,
                NormalizedName = Roles.User.ToUpper()
            }
        };
    }
}