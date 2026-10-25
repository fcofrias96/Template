using Template.BuildingBlocks.Application.Attributes;
using Template.BuildingBlocks.Application.Utilities;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Items
{
    public class ItemQueryParameters : QueryParametersBase<Item>
    {
        [AllowedValues("Id", "Name", "Price", "Description", "Category", null, "Created")]
        public override string? SortBy { get; set; }

        [NumberRangeFormat]
        public string? Price { get; set; }

        public string? Description { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }

        [DateRangeFormat]
        public string? CreatedAt { get; set; } //delete
    }
}
