using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Attributes;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Categories
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
