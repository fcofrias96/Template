

using DocumentFormat.OpenXml.Wordprocessing;

namespace Template.Application.Models.DTOs.V1.Items
{
    public class ItemListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public DateTimeOffset Created { get; set; } // delete

        public bool IsDeleted { get; set; } // delete


    }
}