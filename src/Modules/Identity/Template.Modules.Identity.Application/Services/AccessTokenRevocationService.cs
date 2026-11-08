using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Template.BuildingBlocks.Application.Caching;
using Template.Modules.Identity.Application.Caching;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Application.Settings;

namespace Template.Modules.Identity.Application.Services
{
    public sealed class AccessTokenRevocationService : IAccessTokenRevocationService
    {
        private readonly ICacheService _cache;
        private readonly IUserIdResolver _userIdResolver;

        // A token issued right before the revocation expires after this long, so the entry is not needed afterwards.
        private readonly TimeSpan _revocationTtl;

        public AccessTokenRevocationService(
            ICacheService cache,
            IUserIdResolver userIdResolver,
            IOptions<JwtSettings> jwtSettings)
        {
            _cache = cache;
            _userIdResolver = userIdResolver;
            _revocationTtl = TimeSpan.FromMinutes(jwtSettings.Value.LoginTokenDurationInMinutes);
        }

        public void Revoke(int userId)
        {
            // Whole seconds, like the iat claim it is compared with. A token issued within the same second as the
            // revocation stays valid; the alternative would reject a login done right after a logout.
            var revokedAt = DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _cache.Set(IdentityCacheKeys.RevokedAccessTokens(userId), revokedAt, _revocationTtl);
        }

        public async Task<bool> IsRevokedAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            // JwtBearer maps "sub" to NameIdentifier by default; the raw name covers MapInboundClaims = false.
            var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var userId = await _userIdResolver.GetInternalUserIdAsync(subject, cancellationToken);
            if (userId is null)
            {
                return false;
            }

            var revokedAt = _cache.Get<DateTimeOffset?>(IdentityCacheKeys.RevokedAccessTokens(userId.Value));
            if (revokedAt is null)
            {
                return false;
            }

            // Tokens without iat were issued before revocation existed, hence before this revocation.
            if (!long.TryParse(principal.FindFirstValue(JwtRegisteredClaimNames.Iat), out var issuedAtSeconds))
            {
                return true;
            }

            return DateTimeOffset.FromUnixTimeSeconds(issuedAtSeconds) < revokedAt.Value;
        }
    }
}
