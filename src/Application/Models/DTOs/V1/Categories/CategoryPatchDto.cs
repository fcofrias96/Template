using ServiceStack.DataAnnotations;
using Template.Application.Contracts;
using Template.Application.Contracts.Mapping;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.DTOs.V1.Categories
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