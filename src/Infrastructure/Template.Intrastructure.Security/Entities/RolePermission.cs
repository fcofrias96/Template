using Template.Domain.Common;

namespace Template.Security.Entities
{
    public class RolePermission: EntityBase<int>
	{
		public override int Id { get; set; }
		public int RoleId { get; set; }
        public ApplicationRole Role { get; set; }
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}