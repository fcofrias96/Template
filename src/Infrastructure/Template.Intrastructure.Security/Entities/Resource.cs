using Template.Domain.Common;

namespace Template.Security.Entities
{
    public class Resource : EntityBase<int>
    {
        public override int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ResourceAction> ResourceActions { get; set; } = [];
    }
}