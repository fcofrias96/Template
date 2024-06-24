using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.Item
{
    public class ItemUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }
    }
}