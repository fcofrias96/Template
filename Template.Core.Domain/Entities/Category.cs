using Template.Core.Domain.Common;

namespace Template.Core.Domain.Entities
{
    public class Category : AuditableBaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}