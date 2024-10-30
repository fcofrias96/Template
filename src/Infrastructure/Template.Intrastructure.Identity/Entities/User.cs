using Microsoft.AspNetCore.Identity;
using Template.Domain.Contracts;

namespace Template.Identity.Entities
{
    public class User : IdentityUser, IEntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsTokenRevoked { get; set; }
        public DateTime? TokenRevokedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public bool IsDeleted { get; set; }

        object IEntityBase.Id
        {
            get => Id;
            set => Id = (string)value;
        }

        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}