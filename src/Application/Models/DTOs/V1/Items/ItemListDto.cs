using System.Linq.Expressions;
using Template.Application.Contracts.Mapping;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.DTOs.V1.Items
{
    public class ItemListDto: IProjection<Item, ItemListDto>
    {
        public static Expression<Func<Item, ItemListDto>> Projection => item => new ItemListDto
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            Category = item.Category.Name
        };

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }
}