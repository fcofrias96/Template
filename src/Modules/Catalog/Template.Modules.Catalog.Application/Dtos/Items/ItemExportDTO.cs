using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Attributes;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Items
{
    public class ItemExportDto: IProjection<Item, ItemExportDto>
    {
        public static Expression<Func<Item, ItemExportDto>> Projection => iten=> new ItemExportDto
        {
            Name = iten.Name,
            Category = iten.Category.Name,
            Price = iten.Price,
            Description = iten.Description,
            Active = !iten.IsDeleted,
            CreatedAt = iten.CreatedAt
        };

        [DisplayName("Nombre")]
        public string Name { get; set; }

        [DisplayName("Categoría")]
        public string Category { get; set; }

        [DisplayName("Precio")]
        public decimal Price { get; set; }

        [DisplayName("Descripción")]
        public string Description { get; set; }

        [DisplayName("Activo")]
        public bool Active { get; set; }

        [DisplayName("Creado")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
