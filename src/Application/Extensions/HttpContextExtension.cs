using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Template.Application.Extensions
{
    public static class HttpContextExtension
    {
        public static string GetUserName(this HttpContext httpContext)
        {
            return httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "default";
        }

        public static Guid GetUserId(this HttpContext httpContext)
        {
            var userId = httpContext?.User.FindFirstValue("userId") ?? string.Empty;
            return Guid.Parse(userId);
        }
    }
}