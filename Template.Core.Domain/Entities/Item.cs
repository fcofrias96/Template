namespace Template.Core.Domain.Entities
{
    public class Item : AuditableEntity<Guid>
    {
        public override Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}