namespace Template.Core.Application.DTOs.Item
{
    public class ItemUpdateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}