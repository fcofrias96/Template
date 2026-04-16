
using Template.Application.Contracts.Caching.Invalidators;

namespace Template.Application.Caching.Invalidators
{
    public sealed class MenuCacheInvalidator : IMenuCacheInvalidator
    {
        private readonly ICacheNamespaceVersion _ns;
        private static readonly TimeSpan NsTtl = TimeSpan.FromDays(7);

        public MenuCacheInvalidator(ICacheNamespaceVersion ns) => _ns = ns;

        public void InvalidateMenu() => _ns.Bump(SecurityCacheNamespaces.MenuGlobal, NsTtl);
        public void InvalidatePermissions() => _ns.Bump(SecurityCacheNamespaces.PermissionsGlobal, NsTtl);
    }
}