using Template.BuildingBlocks.Application.Attributes;
using Template.BuildingBlocks.Application.Utilities;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Roles
{
    public class RoleQueryParameters : QueryParametersBase<ApplicationRole>
    {
        [AllowedValues("Name", null)]
        public override string? SortBy { get; set; }

        public string? Name { get; set; }
    }
}
