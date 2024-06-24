namespace Template.Identity.Entities
{
    public class UserModulePermission
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}