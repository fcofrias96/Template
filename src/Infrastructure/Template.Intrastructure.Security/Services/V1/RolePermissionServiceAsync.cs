using Template.Security.Entities;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;

namespace Template.Security.Services.V1
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
                ResourceActionId = permissionId
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