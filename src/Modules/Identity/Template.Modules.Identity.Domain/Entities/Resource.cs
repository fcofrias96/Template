using Template.Modules.Identity.Contracts.Authorization;
using Template.SharedKernel.Domain;

namespace Template.Modules.Identity.Domain.Entities
{
    public class Resource : EntityBase
    {
        public const int User = PermissionResources.User;
        public const int Role = PermissionResources.Role;
        public const int ResourceItem = PermissionResources.ResourceItem;
        public const int Permission = PermissionResources.Permission;
        public const int Action = PermissionResources.Action;
        public const int Configuration = PermissionResources.Configuration;
        public const int Category = PermissionResources.Category;
        public const int Item = PermissionResources.Item;
        
        public string Name { get; set; }

        public ICollection<Permission> Permissions { get; set; } = [];
    }
}
