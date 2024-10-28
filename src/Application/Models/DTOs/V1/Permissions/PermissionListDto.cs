namespace Template.Application.Models.DTOs.V1.Permissions
{
    public class PermissionListDto
    {
        public int Id { get; set; }
        public string Resource { get; set; }
        public string Action { get; set; }
    }
}