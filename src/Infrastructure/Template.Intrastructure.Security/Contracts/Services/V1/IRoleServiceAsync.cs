using Template.Application.Contracts.Services.Base;
using Template.Application.Models.Wrappers;
using Template.Security.Dtos.RolePermission;
using Template.Security.Entities;

namespace Template.Security.Contracts.Services.V1
{
    public interface IRoleServiceAsync : IBaseServiceAsync<Role>
    {
        Task<ApiResponse<IReadOnlyList<RolePermissionDto>>> GetPermissionsAsync(string roleId, CancellationToken cancellationToken);
        Task UpdatePermissionsAsync(string roleId, IEnumerable<int> permissionIds, CancellationToken cancellationToken);
    }

}