using Template.Modules.Identity.Application.Models;
using Microsoft.AspNetCore.Identity;
using Template.BuildingBlocks.Application.Caching;
using Template.Modules.Identity.Application.Caching;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Infrastructure.Services
{
    public class UserAuthCacheService : IUserAuthCacheService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICacheService _cache;
        private readonly TimeSpan _authCacheTtl = TimeSpan.FromHours(1);

        public UserAuthCacheService(UserManager<ApplicationUser> userManager, ICacheService cache)
        {
            _userManager = userManager;
            _cache = cache;
        }

        public async Task<UserAuthCacheEntry> GetUserAuthDataAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var cacheKey = IdentityCacheKeys.UserAuthData(user.Id);
            var cached = _cache.Get<UserAuthCacheEntry>(cacheKey);
            if (cached is not null)
            {
                return cached;
            }

            cancellationToken.ThrowIfCancellationRequested();
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var entry = new UserAuthCacheEntry
            {
                Claims = userClaims.ToList(),
                Roles = userRoles.ToList()
            };

            _cache.Set(cacheKey, entry, _authCacheTtl);
            return entry;
        }
    }
}
