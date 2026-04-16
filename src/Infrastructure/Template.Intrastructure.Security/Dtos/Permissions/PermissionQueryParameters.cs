using Template.Application.Attributes;
using Template.Application.Utilities;
using Template.Domain.Entities.V1;

namespace Template.Security.Dtos.Permissions
{
    public class PermissionQueryParameters : QueryParametersBase<Permission>
    {
        [AllowedValues("Name", "Description", null)]
        public override string? SortBy { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}