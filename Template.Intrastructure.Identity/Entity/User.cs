using Microsoft.AspNetCore.Identity;

namespace Template.Infrastructure.Identity.Entity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}