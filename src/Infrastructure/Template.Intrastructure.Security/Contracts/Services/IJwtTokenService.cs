using System.IdentityModel.Tokens.Jwt;
using Template.Domain.Entities.V1;
using Template.Security.Entities;

namespace Template.Security.Contracts.Services
{
    public interface IJwtTokenService
    {
        Task<(JwtSecurityToken Token, IReadOnlyList<string> Roles)> GenerateTokenAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default);
    }
}