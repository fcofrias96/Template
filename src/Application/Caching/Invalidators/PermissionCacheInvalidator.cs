using Template.Application.Contracts.Caching.Invalidators;

namespace Template.Application.Caching.Invalidators
{
    public sealed class PermissionCacheInvalidator : IPermissionCacheInvalidator
    {
        private readonly ICacheNamespaceVersion _ns;
        private static readonly TimeSpan NsTtl = TimeSpan.FromDays(7);

        public PermissionCacheInvalidator(ICacheNamespaceVersion ns) => _ns = ns;

        public void InvalidateAll()
            => _ns.Bump(SecurityCacheNamespaces.PermissionsGlobal, NsTtl);

        public void InvalidateUser(int userId)
            => _ns.Bump(SecurityCacheNamespaces.PermissionsByUser(userId), NsTtl);
    }
}