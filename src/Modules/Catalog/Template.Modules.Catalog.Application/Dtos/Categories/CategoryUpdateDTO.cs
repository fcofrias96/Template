using ServiceStack.DataAnnotations;
using Template.BuildingBlocks.Application.Contracts;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Categories
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
