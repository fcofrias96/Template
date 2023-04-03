using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;

namespace Template.Infrastructure.Identity.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // Comprueba si el usuario tiene un rol que tiene el permiso requerido
            var userRole = context.User.FindAll(ClaimTypes.Role).Select(claim => claim.Value);
            foreach (var role in userRole)
            {
                if (context.User.HasClaim("Permission", requirement.Permission))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            // Comprueba si el usuario tiene el permiso específico
            if (context.User.HasClaim(claim => claim.Type == "Permission" && claim.Value == requirement.Permission))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}