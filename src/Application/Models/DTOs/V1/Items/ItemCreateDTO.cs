using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.Items
{
    public class ItemCreateDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        [Required]
        public decimal? Price { get; set; }

        [Required]
        public string Description { get; set; }
    }
}