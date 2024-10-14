using Template.Identity.Entities;

namespace Template.Identity.Contracts.Repositories.V1
{
    public interface IRolePermissionRepositoryAsync
    {
        Task AddRanges(RolePermission[] rolePermissions);
        Task RemoveRanges(RolePermission[] rolePermissions);

        Task<RolePermission[]> GetAlls(string roleId, int[] permissionsId);

    }
}