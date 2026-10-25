using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Categories
{
    public class CategoryDetailDto : IProjection<Category, CategoryDetailDto>
    {
        public int Id { get; set; }
        public string Name { get; set; }


        static Expression<Func<Category, CategoryDetailDto>> IProjection<Category, CategoryDetailDto>.Projection => category => new CategoryDetailDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}
