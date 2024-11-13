using Microsoft.AspNetCore.Identity;

namespace Template.Security.Entities
{
    public class UserRole : IdentityUserRole<string>
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}