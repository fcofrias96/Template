using Template.BuildingBlocks.Application.Attributes;
using Template.BuildingBlocks.Application.Utilities;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Users
{
    public class UserQueryParameters : QueryParametersBase<ApplicationUser>
    {
        [AllowedValues("FirstName", "LastName", "UserName", "Email", null)]
        public override string? SortBy { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
