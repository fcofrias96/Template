using System.Security.Claims;

namespace Template.Modules.Identity.Application.Contracts.Services
{
    /// <summary>
    /// Revokes access tokens (JWT) that were already issued. JWTs are stateless, so the revocation is kept in cache
    /// per user and checked on every authenticated request.
    /// </summary>
    public interface IAccessTokenRevocationService
    {
        /// <summary>Invalidates every access token issued to the user up to now. Tokens issued later stay valid.</summary>
        void Revoke(int userId);

        /// <summary>Whether the validated token of <paramref name="principal"/> was issued before a revocation.</summary>
        Task<bool> IsRevokedAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
    }
}
