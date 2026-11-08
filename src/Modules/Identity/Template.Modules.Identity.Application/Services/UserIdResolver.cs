using Template.BuildingBlocks.Application.Caching;
using Template.Modules.Identity.Application.Caching;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Application.Contracts.Services;

namespace Template.Modules.Identity.Application.Services
{
    public sealed class UserIdResolver : IUserIdResolver
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cache;
        private readonly TimeSpan _internalUserIdTtl = TimeSpan.FromDays(1);

        public UserIdResolver(IUserRepository userRepository, ICacheService cache)
        {
            _userRepository = userRepository;
            _cache = cache;
        }

        public async Task<int?> GetInternalUserIdAsync(string? userPublicId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userPublicId, out var publicId))
            {
                return null;
            }

            var cacheKey = IdentityCacheKeys.InternalUserId(publicId);

            var cachedUserId = _cache.Get<int?>(cacheKey);
            if (cachedUserId.HasValue)
            {
                return cachedUserId.Value;
            }

            var userId = await _userRepository.GetIdByPublicIdAsync(publicId, cancellationToken);

            if (userId.HasValue)
            {
                _cache.Set(cacheKey, userId.Value, _internalUserIdTtl);
            }

            return userId;
        }
    }
}
