using Template.Domain.Common;

namespace Template.Identity.Entities
{
    public class Action : EntityBase<int>
    {
        public override int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Permission> Permissions { get; set; } = [];
    }
}