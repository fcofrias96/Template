using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Template.Application.Enums;
using Template.Application.Extensions;
using Template.Security.Contracts.Services.V1;

namespace Template.Api.Attributes
{
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
            var permissionService = httpContext.RequestServices.GetService<IPermissionService>();

            var userIdClaim = httpContext.User.FindFirst("userId");
            if (userIdClaim == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            int userId =  int.Parse(userIdClaim.Value);

            var hasPermission = await permissionService!.HasPermissionAsync(userId, _resourceId, _actionId, httpContext.RequestAborted);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}