namespace Template.Application.Models.DTOs.V1.Permissions
{
    public class PermissionDetailDto
    {
        public int Id { get; set; }
        public string Resource { get; set; }
        public int ResourceId { get; set; }
        public string Action { get; set; }
        public int ActionId { get; set; }
        public string[] Roles { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}