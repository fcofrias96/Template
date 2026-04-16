using Template.Domain.Common;

namespace Template.Domain.Entities.V1
{
    public class Item : EntityBase
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}