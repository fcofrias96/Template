namespace Template.Core.Domain.Entities
{
    public class Category : AuditableEntity<int>
    {
        public override int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Item> Items { get; set; } = new List<Item>();
    }
}