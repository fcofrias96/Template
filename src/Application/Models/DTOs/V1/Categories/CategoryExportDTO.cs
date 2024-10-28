using Template.Application.Attributes;

namespace Template.Application.Models.DTOs.V1.Categories
{
    public class CategoryExportDTO
    {
        [DisplayName("Nombre")]
        public string Name { get; set; }
        [DisplayName("Creado")]
        public DateTime Created { get; set; }
        [DisplayName("Activo")]
        public bool Active { get; set; }
    }
}