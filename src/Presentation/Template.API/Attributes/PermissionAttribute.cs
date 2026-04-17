using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            var hasPermission = await permissionService!.HasPermissionAsync(userIdClaim.Value, _resourceId, _actionId, httpContext.RequestAborted);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}