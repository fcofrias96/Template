using System.Security.Claims;
using Template.BuildingBlocks.Application.Contracts.Services.Base;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Contracts.Services
{
    public interface IPermissionService : IBaseService<Permission>
    {
        Task<bool> HasPermissionAsync(string userPublicId, int resourceId, int actionId, CancellationToken cancellationToken);

        Task<HashSet<int>> GetUserPermissionIdsAsync(ClaimsPrincipal user, CancellationToken cancellationToken);
    }
}
