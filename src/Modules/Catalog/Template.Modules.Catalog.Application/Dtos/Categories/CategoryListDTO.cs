using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Categories
{
    public class CategoryListDto : IProjection<Category, CategoryListDto>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static Expression<Func<Category, CategoryListDto>> Projection => category => new CategoryListDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}
