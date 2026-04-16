using System.Security.Claims;
using Template.Application.Contracts.Services.Base;
using Template.Domain.Entities.V1;

namespace Template.Security.Contracts.Services.V1
{
    public interface IPermissionService : IBaseService<Permission>
    {
        Task<bool> HasPermissionAsync(int userId, int resourceId, int actionId, CancellationToken cancellationToken);

        Task<HashSet<int>> GetUserPermissionIdsAsync(ClaimsPrincipal user, CancellationToken cancellationToken);
    }
}