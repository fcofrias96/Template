using Template.Core.Application.Contracts;
using Template.Core.Application.DTOs.Item;

namespace Template.Core.Application.DTOs.Category
{
    public class CategoryDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ItemListDTO> Items { get; set; } 

    }
}
