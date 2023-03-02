using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.HttpsPolicy;
using WebApi.Middlewares;

namespace WebApi.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void AddMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<CorsMiddleware>("AllowAll");
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<HttpsRedirectionMiddleware>();
            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseMiddleware<SerilogMiddleware>();
            app.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}