using System.Linq.Expressions;
using Template.Application.Attributes;
using Template.Application.Contracts.Mapping;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.DTOs.V1.Items
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