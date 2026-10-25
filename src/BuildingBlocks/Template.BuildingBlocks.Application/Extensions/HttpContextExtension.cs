using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Template.BuildingBlocks.Application.Extensions
{
    public static class HttpContextExtension
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            return httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "default";
        }
    }
}
