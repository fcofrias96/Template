using Template.BuildingBlocks.Application.Attributes;
using Template.BuildingBlocks.Application.Utilities;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Resources
{
    public class ResourceQueryParameters : QueryParametersBase<Resource>
    {
        [AllowedValues("Name", null)]
        public override string? SortBy { get; set; }

        public string? Name { get; set; }
    }
}
