using Microsoft.AspNetCore.Identity;
using Template.SharedKernel.Domain;

namespace Template.Modules.Identity.Domain.Entities
{
    public class ApplicationRole : IdentityRole<int>, IEntityBase
    {
        public string CreatedBy { get; private set; } = AuditDefaults.SystemUser;
        public DateTimeOffset CreatedAt { get; private set; }
        public string? LastModifiedBy { get; private set; }
        public DateTimeOffset? LastModifiedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = [];
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];

        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
        public void MarkDeleted()
        {
            if (IsDeleted) return;
            IsDeleted = true;
        }

        // IdentityUser/IdentityRole are already base classes, so the EntityBase members are repeated here.
        void IAuditable.StampCreated(string userId, DateTimeOffset at)
        {
            CreatedBy = userId;
            CreatedAt = at;
        }

        void IAuditable.StampModified(string userId, DateTimeOffset at)
        {
            LastModifiedBy = userId;
            LastModifiedAt = at;
        }
    }
}
