using Template.Domain.Common;

namespace Template.Domain.Entities.V1
{
    public class Category : EntityBase<int>
    {
        public override int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Item> Items { get; set; } = new List<Item>();
    }
}