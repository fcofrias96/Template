using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Template.Application.Enums;
using Template.Application.Extensions;
using Template.Identity.Contracts.Services.V1;

namespace Template.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly int _resourceId;
        private readonly int _actionId;

        public PermissionAttribute(Resources resource, Actions action)
        {
            _resourceId = resource.ToInt();
            _actionId = action.ToInt();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var permissionService = context.HttpContext.RequestServices.GetService<IPermissionServiceAsync>();

            var userIdClaim = context.HttpContext.User.FindFirst("userId");
            if (userIdClaim == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            string userId = userIdClaim.Value;

            var hasPermission = await permissionService!.HasPermissionAsync(userId, _resourceId, _actionId);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}