using System.Security.Claims;

namespace Template.Security.Contracts.Services.V1
{
    public interface IPermissionService
    {
        HashSet<int> GetUserPermissionIds(ClaimsPrincipal user, CancellationToken cancellationToken);
    }

}