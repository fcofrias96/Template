using Template.Modules.Identity.Application.Dtos.RolePermission;

namespace Template.Modules.Identity.Application.Contracts.Repositories
{
    public interface IRolePermissionRepository
    {
        Task UpdateAsync(int roleId, int[] newPermissionIds, CancellationToken cancellationToken);

        Task AddPermissionsAsync(int roleId, IEnumerable<RolePermissionDetailDto> permissions, CancellationToken cancellationToken);
        
        Task RemovePermissionsAsync(int roleId, IEnumerable<RolePermissionDetailDto> permissions, CancellationToken cancellationToken);
        
        Task<IReadOnlyList<RolePermissionDto>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken cancellationToken);
    }
}
