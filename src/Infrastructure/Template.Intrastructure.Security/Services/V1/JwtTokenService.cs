using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Template.Domain.Entities.V1;
using Template.Security.Contracts.Services;
using Template.Security.Entities;
using Template.Security.Settings;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly SigningCredentials _signingCredentials;
    private readonly IUserAuthCacheService _userAuthCacheService;

    public JwtTokenService(
        IOptions<JwtSettings> jwtSettings,
        IUserAuthCacheService userAuthCacheService)
    {
        _jwtSettings = jwtSettings.Value;
        _userAuthCacheService = userAuthCacheService;

        var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var symmetricSecurityKey = new SymmetricSecurityKey(keyBytes);
        _signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
    }

    public async Task<(JwtSecurityToken Token, IReadOnlyList<string> Roles)> GenerateTokenAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        var authData = await _userAuthCacheService.GetUserAuthDataAsync(user, cancellationToken);
        var claims = user.GenerateClaims();

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.LoginTokenDurationInMinutes),
            signingCredentials: _signingCredentials);

        return (jwtSecurityToken, authData.Roles);
    }
}
