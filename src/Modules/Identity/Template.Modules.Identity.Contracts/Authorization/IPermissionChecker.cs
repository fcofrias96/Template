namespace Template.Modules.Identity.Contracts.Authorization
{
    /// <summary>
    /// Cross-module permission check. Implemented by the Identity module and consumed by
    /// <see cref="PermissionAttribute"/> in any module's controllers.
    /// </summary>
    public interface IPermissionChecker
    {
        Task<bool> HasPermissionAsync(string userPublicId, int resourceId, int actionId, CancellationToken cancellationToken);
    }
}
