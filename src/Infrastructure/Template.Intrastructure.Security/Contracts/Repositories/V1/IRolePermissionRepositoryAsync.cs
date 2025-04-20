using Template.Security.Entities;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IRolePermissionRepositoryAsync
    {

        Task<RolePermission[]> GetAlls(string roleId, int[] permissionsId);

        Task UpdateAsync(string roleId, int[] newPermissionIds);
    }
}