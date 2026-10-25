using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Template.Modules.Identity.Contracts.Authorization
{
    /// <summary>
    /// Requires the current user to hold the given resource/action permission.
    /// Usable from any module: it depends only on <see cref="IPermissionChecker"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly int _resourceId;
        private readonly int _actionId;

        public PermissionAttribute(int resource, int action)
        {
            _resourceId = resource;
            _actionId = action;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var permissionChecker = httpContext.RequestServices.GetService<IPermissionChecker>();

            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            var hasPermission = await permissionChecker!.HasPermissionAsync(userIdClaim.Value, _resourceId, _actionId, httpContext.RequestAborted);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
