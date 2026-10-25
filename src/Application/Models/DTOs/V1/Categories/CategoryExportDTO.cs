using System.Linq.Expressions;
using Template.Application.Attributes;
using Template.Application.Contracts.Mapping;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.DTOs.V1.Categories
{
    public class CategoryExportDto : IProjection<Category, CategoryExportDto>
    {
        public static Expression<Func<Category, CategoryExportDto>> Projection => category => new CategoryExportDto
        {
            Name = category.Name,
            CreatedAt = category.CreatedAt,
            Active = !category.IsDeleted
        };

        [DisplayName("Nombre")]
        public string Name { get; set; }

        [DisplayName("Creado")]
        public DateTimeOffset CreatedAt { get; set; }

        [DisplayName("Activo")]
        public bool Active { get; set; }
    }
}