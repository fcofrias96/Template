using Microsoft.AspNetCore.Identity;
using Template.Domain.Contracts;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Template.Security.Entities
{
    public class ApplicationUser : IdentityUser<int>, IEntityBase
    {
        public Guid PublicId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RefreshToken { get; set; }
        public DateTimeOffset RefreshTokenExpiryTime { get; set; }
        public bool IsTokenRevoked { get; set; }
        public DateTimeOffset? TokenRevokedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
        public void MarkDeleted()
        {
            if (IsDeleted) return;
            IsDeleted = true;
        }

        public IEnumerable<Claim> GenerateClaims()
        {
            yield return new Claim(JwtRegisteredClaimNames.Sub, PublicId.ToString());
            yield return new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
        }
    }
}