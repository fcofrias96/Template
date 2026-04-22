using ServiceStack.DataAnnotations;
using Template.Application.Contracts;

namespace Template.Application.Models.DTOs.V1.Categories
{
    public sealed class CategoryUpdateDto : IUpdateDto
    {
        [Range(1, int.MaxValue)]
        public int Id { get; init; }

        [Required]
        public required string Name { get; init; }
    }
}