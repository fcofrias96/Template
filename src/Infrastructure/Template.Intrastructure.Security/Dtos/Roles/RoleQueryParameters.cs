using Template.Application.Attributes;
using Template.Application.Utilities;
using Template.Security.Entities;

namespace Template.Security.Dtos.Roles
{
    public class RoleQueryParameters : QueryParametersBase<Role>
    {
        [AllowedValues("Name", null)]
        public override string? SortBy { get; set; }

        public string? Name { get; set; }
    }
}