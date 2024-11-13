using Template.Application.Constants;
using Template.Security.Entities;

namespace Template.Security.Seeds
{
    public static class RolePermissionSeed
    {
        public static List<RolePermission> Data { get; } = InitializeData();

        private static List<RolePermission> InitializeData()
        {
            var data = new List<RolePermission>
        {
            #region Admin

            new RolePermission
            {
                RoleId = RoleSeed.Data.First(x => x.Name == Roles.User).Id,
                ResourceActionId = 23
            },
            new RolePermission
            {
                RoleId = RoleSeed.Data.First(x => x.Name == Roles.User).Id,
                ResourceActionId = 28
            },

            #endregion Admin
        };

            data.AddRange(GetAdminRolePermissions());

            return data;
        }

        private static List<RolePermission> GetAdminRolePermissions()
        {
            var rolePermissions = new List<RolePermission>();
            var roleId = RoleSeed.Data.First(x => x.Name == Roles.Admin).Id;

            for (var index = 1; index <= 32; index++)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    ResourceActionId = index
                });
            }

            return rolePermissions;
        }
    }
}