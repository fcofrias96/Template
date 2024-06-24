using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Template.Identity.Contracts.Services.V1;

namespace Template.Identity.Authorization
{
    public class ModulePermissionHandler : AuthorizationHandler<ModulePermissionRequirement>
    {
        private readonly IServiceProvider _serviceProvider;

        public ModulePermissionHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ModulePermissionRequirement requirement)
        {
            if (context.Resource is not HttpContext httpContext)
            {
                context.Fail();
                return;
            }

            var controllerName = httpContext.GetRouteData().Values["controller"]?.ToString() ?? string.Empty;
            var userId = httpContext.User.FindFirst("userId")?.Value ?? string.Empty;

            bool isAuthorized = await IsUserAuthorizedAsync(userId, controllerName, requirement.Permission);

            if (isAuthorized)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }

        private async Task<bool> IsUserAuthorizedAsync(string userId, string controllerName, string permission)
        {
            using var scope = _serviceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserServiceAsync>();
            return await userService.IsAuthorized(userId, controllerName, permission);
        }
    }
}