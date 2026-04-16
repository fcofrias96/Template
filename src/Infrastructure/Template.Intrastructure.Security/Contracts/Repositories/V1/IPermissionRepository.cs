using System.Security.Claims;
using Template.Application.Contracts.Repositories.Base;
using Template.Security.DbContexts.V1;
using Template.Security.Dtos.UserPermission;
using Template.Security.Entities;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IPermissionRepository : IBaseRepository<Permission>
    {
        Task<IReadOnlyList<UserPermissionDto>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken);

        Task<IReadOnlyList<int>> GetUserPermissionIdsAsync(int userId, CancellationToken cancellationToken);

    }

}