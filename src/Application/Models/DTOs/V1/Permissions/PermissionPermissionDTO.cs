namespace Template.Application.Models.DTOs.V1.Permissions
{
    public class PermissionPermissionDto
    {
        public int Id { get; set; }
        public string Permission { get; set; }
        public string[] Permissions { get; set; }
    }
}