using System.ComponentModel.DataAnnotations;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Items
{
    public class ItemCreateDto: ICreateMapTo<Item>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        public Item ToEntity()
        {
            return new Item
            {
                Name = Name,
                CategoryId = CategoryId,
                Price = Price,
                Description = Description
            };
        }
    }
}
