using Template.Application.Constants;
using Template.Security.Entities;

namespace Template.Security.Seeds
{
    public static class RoleSeed
    {
        public static List<Role> Data { get; } = new List<Role>
        {
            new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = Roles.Admin,
                NormalizedName = Roles.Admin.ToUpper()
            },
            new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = Roles.User,
                NormalizedName = Roles.User.ToUpper()
            }
        };
    }
}