using System.Security.Claims;
using Template.Application.Contracts.Repositories.Base;
using Template.Domain.Entities.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Dtos.UserPermission;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IPermissionRepository : IBaseRepository<Permission>
    {
        Task<IReadOnlyList<UserPermissionDto>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken);

        Task<IReadOnlyList<int>> GetUserPermissionIdsAsync(int userId, CancellationToken cancellationToken);

    }

}