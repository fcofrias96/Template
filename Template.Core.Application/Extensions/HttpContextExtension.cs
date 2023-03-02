using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Template.Core.Application.Extensions
{
    public static class HttpContextExtension
    {
        public static string GetUserName(this HttpContext httpContext)
        {
            return httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "default";
        }
    }
}