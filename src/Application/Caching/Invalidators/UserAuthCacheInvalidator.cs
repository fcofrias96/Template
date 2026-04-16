
using Template.Application.Contracts.Caching.Invalidators;

namespace Template.Application.Caching.Invalidators
{
    public sealed class UserAuthCacheInvalidator : IUserAuthCacheInvalidator
    {
        private readonly ICacheNamespaceVersion _ns;
        private static readonly TimeSpan NsTtl = TimeSpan.FromDays(7);

        public UserAuthCacheInvalidator(ICacheNamespaceVersion ns) => _ns = ns;

        public void InvalidateAll()
            => _ns.Bump(SecurityCacheNamespaces.AuthGlobal, NsTtl);

        public void InvalidateUser(int userId)
            => _ns.Bump(SecurityCacheNamespaces.AuthByUser(userId), NsTtl);
    }
}