using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Entities;

namespace Template.Identity.Services.V1
{
    public class RolePermissionServiceAsync : IRolePermissionServiceAsync
    {
        private readonly IRolePermissionRepositoryAsync _rolePermissionRepositoryAsync;

        public RolePermissionServiceAsync(IRolePermissionRepositoryAsync rolePermissionRepositoryAsync)
        {
            _rolePermissionRepositoryAsync = rolePermissionRepositoryAsync;
        }

        public async Task AddPermissionToRoleAsync(Guid roleId, int[] permissionsId)
        {
            var rolePermissions = permissionsId.Select(permissionId => new RolePermission
            {
                RoleId = roleId.ToString(),
                PermissionId = permissionId
            }).ToArray();

            await _rolePermissionRepositoryAsync.AddRanges(rolePermissions);
        }

        public async Task RemovePermissionToRoleAsync(Guid roleId, int[] permissionsId)
        {
            var rolePermissions = await _rolePermissionRepositoryAsync.GetAlls(roleId.ToString(), permissionsId);
            await _rolePermissionRepositoryAsync.RemoveRanges(rolePermissions);
        }
    }
}