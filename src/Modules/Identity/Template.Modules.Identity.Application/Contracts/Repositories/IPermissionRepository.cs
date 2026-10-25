using System.Security.Claims;
using Template.BuildingBlocks.Application.Contracts.Repositories.Base;
using Template.Modules.Identity.Application.Dtos.UserPermission;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Contracts.Repositories
{
    public interface IPermissionRepository : IBaseRepository<Permission>
    {
        Task<IReadOnlyList<UserPermissionDto>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken);

        Task<IReadOnlyList<int>> GetUserPermissionIdsAsync(int userId, CancellationToken cancellationToken);

    }

}
