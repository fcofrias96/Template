using Template.Core.Domain.Common;

namespace Template.Core.Domain.Entities
{
    public class Item : AuditableBaseEntity
    {
        public Guid Id { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; }
        public Category Category { get; set; }

    }
}