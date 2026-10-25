using System.Linq.Expressions;
using Template.Application.Contracts.Mapping;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.DTOs.V1.Categories
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