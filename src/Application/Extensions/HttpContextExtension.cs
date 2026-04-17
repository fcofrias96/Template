using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Template.Application.Extensions
{
    public static class HttpContextExtension
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            return httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "default";
        }
    }
}