using Template.Security.Entities;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IRolePermissionRepositoryAsync
    {
        Task<RolePermission[]> GetAllAsync(string roleId, int[] permissionsId, CancellationToken cancellationToken);

        Task UpdateAsync(string roleId, int[] newPermissionIds, CancellationToken cancellationToken);
    }
}