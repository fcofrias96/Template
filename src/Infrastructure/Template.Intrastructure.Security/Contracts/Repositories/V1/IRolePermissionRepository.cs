using Template.Application.Contracts.Repositories.Base;
using Template.Security.Dtos.RolePermission;
using Template.Security.Entities;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IRolePermissionRepository : IBaseRepository<RolePermission>
    {
        Task UpdateAsync(int roleId, int[] newPermissionIds, CancellationToken cancellationToken);

        Task AddPermissionsAsync(int roleId, IEnumerable<RolePermissionDetailDto> permissions, CancellationToken cancellationToken);
        
        Task RemovePermissionsAsync(int roleId, IEnumerable<RolePermissionDetailDto> permissions, CancellationToken cancellationToken);
        
        Task<IReadOnlyList<RolePermissionDto>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken cancellationToken);
    }
}