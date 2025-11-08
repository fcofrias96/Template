using Microsoft.AspNetCore.Identity;
using Template.Domain.Contracts;

namespace Template.Security.Entities
{
    public class Role : IdentityRole<string>, IEntityBase
    {
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
        public bool IsDeleted { get; set; }

        object IEntityBase.Id
        {
            get => Id;
            set => Id = (string)value;
        }

        public ICollection<RolePermission> RolePermissions { get; set; } = [];
        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}