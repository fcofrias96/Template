namespace Template.Application.Models.DTOs.V1.Item
{
    public class ItemDetailDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}