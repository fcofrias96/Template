using System.Linq.Expressions;
using Template.Application.Contracts.Mapping;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.DTOs.V1.Categories
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