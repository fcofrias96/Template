using System.ComponentModel.DataAnnotations;
using Template.Application.Contracts;

namespace Template.Application.Models.DTOs.V1.Items
{
    public sealed class ItemUpdateDto : IUpdateDto
    {
        [Required]
        public int Id { get; init; }

        [Required]
        public string Name { get; init; } = null!;

        [Range(1, int.MaxValue)]
        public int CategoryId { get; init; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; init; }

        [Required]
        public string Description { get; init; } = null!;
    }
}