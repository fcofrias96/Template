namespace Template.Security.Entities
{
    public class RolePermission
    {
        public string RoleId { get; set; }
        public Role Role { get; set; }
        public int ResourceActionId { get; set; }
        public ResourceAction ResourceAction { get; set; }
    }
}