using System.Security.Claims;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;

namespace Template.Security.Services.V1
{
    public class PermissionService : IPermissionService
    {
        private readonly SecurityDbContext _context;

        public PermissionService(SecurityDbContext context)
        {
            _context = context;
        }

        public HashSet<int> GetUserResourceActionIds(ClaimsPrincipal user)
        {
            if(user == null ||  !user.Identity.IsAuthenticated) { return new HashSet<int>(); }
            
            var userId = GetUserIdFromClaims(user);

            return _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.ResourceActionId)
                .ToHashSet();

        }

        private static string GetUserIdFromClaims(ClaimsPrincipal user)
        {
            return user.FindFirst("UserId").Value;
        }
    }

}