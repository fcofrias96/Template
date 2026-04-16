using Template.Application.Attributes;
using Template.Application.Utilities;
using Template.Domain.Entities.V1;

namespace Template.Security.Dtos.Resources
{
    public class PermissionQueryParameters : QueryParametersBase<Resource>
    {
        [AllowedValues("Name", null)]
        public override string? SortBy { get; set; }

        public string? Name { get; set; }
    }
}