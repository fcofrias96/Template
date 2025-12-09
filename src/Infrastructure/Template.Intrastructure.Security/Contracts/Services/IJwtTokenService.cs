using System.IdentityModel.Tokens.Jwt;
using Template.Security.Entities;

public interface IJwtTokenService
{
    Task<(JwtSecurityToken Token, IReadOnlyList<string> Roles)> GenerateTokenAsync(
        User user,
        CancellationToken cancellationToken = default);
}
