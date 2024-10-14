using Microsoft.AspNetCore.Identity;
using Template.Domain.Contracts;

namespace Template.Identity.Entities
{
    public class Role : IdentityRole<string>, IEntityBase
    {
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
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
        public ICollection<UserRole> UserRoles { get; set; } = [];
    }


}