using Template.Application.Attributes;
using Template.Application.Utilities;
using Template.Domain.Entities.V1;
using Template.Security.Entities;

namespace Template.Security.Dtos.Roles
{
    public class RoleQueryParameters : QueryParametersBase<ApplicationRole>
    {
        [AllowedValues("Name", null)]
        public override string? SortBy { get; set; }

        public string? Name { get; set; }
    }
}