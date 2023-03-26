namespace Template.Core.Application.DTOs.Item
{

    public class ItemListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
    }
}
