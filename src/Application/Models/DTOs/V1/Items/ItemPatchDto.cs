using System.ComponentModel.DataAnnotations;
using Template.Application.Contracts;

namespace Template.Application.Models.DTOs.V1.Items
{
    public sealed class ItemPatchDto : IPatchDto
    {
        [Required]
        public int Id { get; init; }

        public string? Name { get; init; }

        [Range(1, int.MaxValue)]
        public int? CategoryId { get; init; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; init; }

        public string? Description { get; init; }
    }
}