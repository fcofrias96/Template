using ServiceStack.DataAnnotations;
using Template.Application.Contracts;
using Template.Application.Contracts.Mapping;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.DTOs.V1.Categories
{
    public sealed class CategoryUpdateDto : IHasId, IUpdateMapTo<Category>
    {
        [Range(1, int.MaxValue)]
        public int Id { get; init; }

        [Required]
        public required string Name { get; init; }

        void IUpdateMapTo<Category>.ApplyTo(Category entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            entity.Name = Name;
        }
    }
}