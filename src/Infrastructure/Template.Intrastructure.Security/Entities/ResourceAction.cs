using Template.Domain.Common;

namespace Template.Security.Entities
{
    public class ResourceAction : EntityBase<int>
    {
        public override int Id { get; set; }
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
        public int ActionId { get; set; }
        public ActionType Action { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}