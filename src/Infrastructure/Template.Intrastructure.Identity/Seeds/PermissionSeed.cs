using Template.Application.Constants;
using Template.Identity.Entities;

namespace Template.Identity.Seeds
{
    public static class PermissionSeed
    {
        public static List<Permission> Data { get; } = new List<Permission>
        {
             new Permission
             {
                Id = 1,
                Name = Permissions.Create
             },
             new Permission
             {
                 Id = 2,
                Name = Permissions.Read
             },
             new Permission
             {
                Id = 3,
                Name = Permissions.Update
             },
             new Permission
             {
                 Id = 4,
                Name = Permissions.Delete
             },
             new Permission
             {
                 Id = 5,
                Name = Permissions.Export
             },
             new Permission
             {
                 Id = 6,
                Name = Permissions.All
             }
        };
    }
}