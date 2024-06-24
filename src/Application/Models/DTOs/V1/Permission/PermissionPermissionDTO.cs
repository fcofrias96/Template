namespace Template.Application.Models.DTOs.V1.Permission
{
    public class PermissionPermissionDTO
    {
        public int Id { get; set; }
        public string Permission { get; set; }
        public List<string> Permissions { get; set; }
    }
}