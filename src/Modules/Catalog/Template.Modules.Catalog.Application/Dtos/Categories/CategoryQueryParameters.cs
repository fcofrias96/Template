using Template.BuildingBlocks.Application.Attributes;
using Template.BuildingBlocks.Application.Utilities;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Categories
{
    public class CategoryQueryParameters : QueryParametersBase<Category>
    {
        [AllowedValues("Name", "Id", null)]
        public override string? SortBy { get; set; }

        public string? Name { get; set; }
    }
}
