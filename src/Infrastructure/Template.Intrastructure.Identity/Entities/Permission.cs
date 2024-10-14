using Template.Domain.Common;

namespace Template.Identity.Entities
{
    public class Permission : EntityBase<int>
    {
        public override int Id { get; set; }
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
        public int ActionId { get; set; }
        public Action Action { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}