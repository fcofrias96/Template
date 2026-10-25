using Template.SharedKernel.Domain;

namespace Template.Modules.Identity.Domain.Entities
{
    public class Permission : EntityBase
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int ResourceId { get; set; }
        public int ActionId { get; set; }
        public virtual Resource Resource { get; set; }

        public virtual PermissionAction Action { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
