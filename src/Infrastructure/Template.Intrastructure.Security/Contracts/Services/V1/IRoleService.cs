using Template.Application.Contracts.Services.Base;
using Template.Application.Models.Wrappers;
using Template.Domain.Entities.V1;
using Template.Security.Dtos.RolePermission;
using Template.Security.Entities;

namespace Template.Security.Contracts.Services.V1
{
    public interface IRoleService : IBaseService<ApplicationRole>
    {
        Task<ApiResponse<IReadOnlyList<RolePermissionDto>>> GetPermissionsAsync(int roleId, CancellationToken cancellationToken);
        Task UpdatePermissionsAsync(int roleId, IEnumerable<int> permissionIds, CancellationToken cancellationToken);
    }

}