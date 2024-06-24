using Microsoft.AspNetCore.Identity;

namespace Template.Identity.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Created { get; set; }
        public bool Active { get; set; }

        public ICollection<UserModulePermission> UserModulePermissions { get; set; }
    }
}