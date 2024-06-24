namespace Template.Application.Models.DTOs.V1.Item
{
    public class ItemListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }
}