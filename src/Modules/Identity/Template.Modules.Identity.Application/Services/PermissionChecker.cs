using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Contracts.Authorization;

namespace Template.Modules.Identity.Application.Services
{
    /// <summary>
    /// Exposes <see cref="IPermissionService.HasPermissionAsync"/> to other modules through the public contract.
    /// </summary>
    public sealed class PermissionChecker : IPermissionChecker
    {
        private readonly IPermissionService _permissionService;

        public PermissionChecker(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public Task<bool> HasPermissionAsync(string userPublicId, int resourceId, int actionId, CancellationToken cancellationToken)
            => _permissionService.HasPermissionAsync(userPublicId, resourceId, actionId, cancellationToken);
    }
}
