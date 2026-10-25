using Template.Modules.Identity.Contracts.Authorization;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Infrastructure.Seeds
{
    public static class RolePermissionSeed
    {
        public static List<RolePermission> Data { get; } = InitializeData();

        private static List<RolePermission> InitializeData()
        {
            var data = new List<RolePermission>
            { 
                // User
                new RolePermission
                {
                    RoleId = RoleSeed.Data.First(x => x.Name == Roles.User).Id,
                    PermissionId = PermissionSeed.Data.First(p => p.ActionId == PermissionAction.Read && p.ResourceId == Resource.Category).Id
                },
                new RolePermission
                {
                    RoleId = RoleSeed.Data.First(x => x.Name == Roles.User).Id,
                    PermissionId = PermissionSeed.Data.First(p => p.ActionId == PermissionAction.Read && p.ResourceId == Resource.Item).Id
                },
            };

            // Admin
            data.AddRange(GetAdminRolePermissions());
 
            return data;
        }

        private static List<RolePermission> GetAdminRolePermissions()
        {
            var rolePermissions = new List<RolePermission>();
            var roleId = RoleSeed.Data.First(x => x.Name == Roles.Admin).Id;

            for (var index = 1; index <= PermissionSeed.Data.Count; index++)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = index
                });
            }

            return rolePermissions;
        }
    }
}
