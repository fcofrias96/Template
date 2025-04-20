using Template.Domain.Common;

namespace Template.Security.Entities
{
    public class ActionType : EntityBase<int>
    {
        public override int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Permission> Permissions { get; set; } = [];
    }
}