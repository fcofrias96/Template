using System.ComponentModel.DataAnnotations;
using Template.BuildingBlocks.Application.Contracts;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Items
{
    public sealed class ItemUpdateDto : IHasId, IUpdateMapTo<Item>
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

        public void ApplyTo(Item entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            entity.Name = Name;
            entity.CategoryId = CategoryId;
            entity.Price = Price;
            entity.Description = Description;
        }
    }
}
