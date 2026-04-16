using Template.Application.Contracts.Caching.Invalidators;

namespace Template.Application.Caching.Invalidators
{
    public sealed class OwnerProfileCacheInvalidator : IOwnerProfileCacheInvalidator
    {
        private readonly ICacheNamespaceVersion _ns;
        private static readonly TimeSpan NsTtl = TimeSpan.FromDays(7);

        public OwnerProfileCacheInvalidator(ICacheNamespaceVersion ns) => _ns = ns;

        public void InvalidateAll()
            => _ns.Bump(SecurityCacheNamespaces.OwnerProfileGlobal, NsTtl);

        public void InvalidateUser(int userId)
            => _ns.Bump(SecurityCacheNamespaces.OwnerProfileByUser(userId), NsTtl);
    }
}