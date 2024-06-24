using Template.Application.Utilities;

namespace Template.Identity.Specification.V1
{
    public class UserFilterRequest : QueryParametersBase
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}