using Template.Application.Attributes;

namespace Template.Application.Models.DTOs.V1.Items
{
    public class ItemExportDto
    {
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