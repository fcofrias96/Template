using System.IdentityModel.Tokens.Jwt;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Contracts.Services
{
    public interface IJwtTokenService
    {
        Task<(JwtSecurityToken Token, IReadOnlyList<string> Roles)> GenerateTokenAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default);
    }
}
