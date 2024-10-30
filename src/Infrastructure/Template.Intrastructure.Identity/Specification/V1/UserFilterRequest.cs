using Template.Application.Utilities;
using Template.Identity.Entities;

namespace Template.Identity.Specification.V1
{
    public class UserFilterRequest : QueryParametersBase<User>
    {
        public override string? SortBy { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}