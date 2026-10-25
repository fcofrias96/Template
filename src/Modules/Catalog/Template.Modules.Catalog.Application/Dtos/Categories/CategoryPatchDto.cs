using ServiceStack.DataAnnotations;
using Template.BuildingBlocks.Application.Contracts;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Categories
{
    public sealed class CategoryPatchDto : IHasId, IUpdateMapTo<Category>
    {
        [Range(1, int.MaxValue)]
        public int Id { get; init; }

        public string? Name { get; init; }

        public void ApplyTo(Category entity)
        {
            ArgumentNullException.ThrowIfNull(entity);


            if (Name is not null)
            {
                entity.Name = Name;
            }
        }
    }
}
