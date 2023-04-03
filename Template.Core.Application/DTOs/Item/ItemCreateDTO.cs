using System.ComponentModel.DataAnnotations;

namespace Template.Core.Application.DTOs.Item
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