using System.Linq.Expressions;
using Template.Application.Contracts.Mapping;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.DTOs.V1.Items
{
    public class ItemDetailDto: IProjection<Item, ItemDetailDto>
    {
        public static Expression<Func<Item, ItemDetailDto>> Projection => item => new ItemDetailDto
        {
            Id = item.Id,
            Name = item.Name,
            CategoryId = item.CategoryId,
            Category = item.Category.Name,
            Price = item.Price,
            Description = item.Description
        };

        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}