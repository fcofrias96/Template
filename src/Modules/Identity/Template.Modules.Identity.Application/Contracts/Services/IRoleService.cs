using Template.BuildingBlocks.Application.Contracts.Services.Base;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Application.Dtos.RolePermission;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Contracts.Services
{
    public interface IRoleService : IBaseService<ApplicationRole>
    {
        Task<ApiResponse<IReadOnlyList<RolePermissionDto>>> GetPermissionsAsync(int roleId, CancellationToken cancellationToken);
        Task UpdatePermissionsAsync(int roleId, IEnumerable<int> permissionIds, CancellationToken cancellationToken);
    }

}
