using ServiceStack.DataAnnotations;
using Template.Application.Contracts;

namespace Template.Application.Models.DTOs.V1.Categories
{
    public sealed class CategoryPatchDto : IPatchDto
    {
        [Range(1, int.MaxValue)]
        public int Id { get; init; }

        public string? Name { get; init; }
    }
}