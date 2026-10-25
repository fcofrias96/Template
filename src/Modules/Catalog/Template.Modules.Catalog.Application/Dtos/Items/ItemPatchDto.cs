using System.ComponentModel.DataAnnotations;
using Template.BuildingBlocks.Application.Contracts;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Items
{
    public sealed class ItemPatchDto : IHasId, IUpdateMapTo<Item>
    {
        [Required]
        public int Id { get; init; }

        public string? Name { get; init; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; init; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; init; }

        public string? Description { get; init; }

        public void ApplyTo(Item entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            entity.CategoryId = CategoryId;

            if (Name is not null)
                entity.Name = Name;
            
            if (Price is not null)
                entity.Price = Price.Value;

            if (Description is not null)
                entity.Description = Description;
        }
    }
}
