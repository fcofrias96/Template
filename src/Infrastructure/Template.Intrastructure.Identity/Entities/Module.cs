using Template.Domain.Common;

namespace Template.Identity.Entities
{
    public class Module : EntityBase<int>
    {
        public override int Id { get; set; }

        public string Name { get; set; }

        public ICollection<UserModulePermission> UserModulePermissions { get; set; }
    }
}