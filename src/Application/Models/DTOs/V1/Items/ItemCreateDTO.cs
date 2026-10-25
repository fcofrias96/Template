using System.ComponentModel.DataAnnotations;
using Template.Application.Contracts.Mapping;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.DTOs.V1.Items
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