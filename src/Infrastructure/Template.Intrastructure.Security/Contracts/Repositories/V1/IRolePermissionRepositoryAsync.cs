using Template.Security.Dtos.RolePermission;
using Template.Security.Entities;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IRolePermissionRepositoryAsync
    {
        Task UpdateAsync(string roleId, int[] newPermissionIds, CancellationToken cancellationToken);

        Task AddPermissionsAsync(string roleId, IEnumerable<RolePermissionDetailDto> permissions, CancellationToken cancellationToken);
        
        Task RemovePermissionsAsync(string roleId, IEnumerable<RolePermissionDetailDto> permissions, CancellationToken cancellationToken);
        
        Task<IReadOnlyList<RolePermissionDto>> GetPermissionsByRoleIdAsync(string roleId, CancellationToken cancellationToken);
    }
}