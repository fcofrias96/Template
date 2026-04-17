using System.Security.Claims;
using Template.Application.Contracts.Services.Base;
using Template.Security.Entities;

namespace Template.Security.Contracts.Services.V1
{
    public interface IPermissionService : IBaseService<Permission>
    {
        Task<bool> HasPermissionAsync(string userPublicId, int resourceId, int actionId, CancellationToken cancellationToken);

        Task<HashSet<int>> GetUserPermissionIdsAsync(ClaimsPrincipal user, CancellationToken cancellationToken);
    }
}