using Template.BuildingBlocks.Application.Attributes;
using Template.BuildingBlocks.Application.Utilities;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Permissions
{
    public class PermissionQueryParameters : QueryParametersBase<Permission>
    {
        [AllowedValues("Name", "Description", null)]
        public override string? SortBy { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
