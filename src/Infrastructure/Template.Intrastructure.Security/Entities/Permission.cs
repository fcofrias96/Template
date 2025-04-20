using Template.Domain.Common;

namespace Template.Security.Entities
{
    public class Permission : EntityBase<int>
    {
        public override int Id { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public int ResourceId { get; set; }
        public int ActionTypeId { get; set; }
        public virtual Resource Resource { get; set; }

        public virtual ActionType ActionType { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}