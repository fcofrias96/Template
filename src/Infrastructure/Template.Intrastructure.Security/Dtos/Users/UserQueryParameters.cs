using Template.Application.Attributes;
using Template.Application.Utilities;
using Template.Domain.Entities.V1;
using Template.Security.Entities;

namespace Template.Security.Dtos.Users
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