using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;
using Template.Security.Entities;
using Template.Security.Settings;

public class AuthLinkService : IAuthLinkService
{
    private readonly IHttpContextAccessor _httpContext;
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<User> _userManager;

    public AuthLinkService(
        IHttpContextAccessor httpContext,
        IOptions<JwtSettings> jwtSettings,
        UserManager<User> userManager)
    {
        _httpContext = httpContext;
        _jwtSettings = jwtSettings.Value;
        _userManager = userManager;
    }

    private string GetApiBaseUrl()
    {
        var httpRequest = _httpContext?.HttpContext?.Request;
        var currentRequestUrl = httpRequest != null ? httpRequest.GetEncodedUrl() : string.Empty;

        return currentRequestUrl.Substring(
            0,
            currentRequestUrl.LastIndexOf("/auth", StringComparison.OrdinalIgnoreCase) + 5);
    }

    public async Task<string> GenerateEmailConfirmationLinkAsync(User user)
    {
        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(emailConfirmationToken));

        var baseUrl = $"{GetApiBaseUrl()}/confirm-email";

        var url = QueryHelpers.AddQueryString(baseUrl, "userId", user.Id);
        url = QueryHelpers.AddQueryString(url, "token", encodedToken);

        return url;
    }

    public async Task<string> GenerateChangeEmailLinkAsync(
        User user,
        string newEmail,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var changeEmailToken = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(changeEmailToken));

        var baseUrl = $"{GetApiBaseUrl()}/confirm-change-email";

        var url = QueryHelpers.AddQueryString(baseUrl, "userId", user.Id);
        url = QueryHelpers.AddQueryString(url, "token", encodedToken);
        url = QueryHelpers.AddQueryString(url, "newEmail", newEmail);

        return url;
    }

    public Task<string> GenerateResetPasswordLinkAsync(User user, string encodedToken)
    {
        var baseUrl = $"{_jwtSettings.Audience}/reset-password";

        var url = QueryHelpers.AddQueryString(baseUrl, "token", encodedToken);
        url = QueryHelpers.AddQueryString(url, "userId", user.Id);

        return Task.FromResult(url);
    }
}
