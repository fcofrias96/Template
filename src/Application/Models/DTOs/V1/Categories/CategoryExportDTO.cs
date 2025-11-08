using Template.Application.Attributes;

namespace Template.Application.Models.DTOs.V1.Categories
{
    public class CategoryExportDto
    {
        [DisplayName("Nombre")]
        public string Name { get; set; }

        [DisplayName("Creado")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Activo")]
        public bool Active { get; set; }
    }
}