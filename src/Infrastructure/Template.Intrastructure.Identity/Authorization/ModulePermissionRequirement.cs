using Microsoft.AspNetCore.Authorization;

namespace Template.Identity.Authorization
{
    public class ModulePermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public ModulePermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}