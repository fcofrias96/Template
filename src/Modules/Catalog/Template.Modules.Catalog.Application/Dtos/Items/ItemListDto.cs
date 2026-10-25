using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Items
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
