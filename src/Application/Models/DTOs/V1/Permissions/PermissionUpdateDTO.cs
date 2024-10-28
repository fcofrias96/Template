namespace Template.Application.Models.DTOs.V1.Permissions
{
    public class PermissionUpdateDTO
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public int ResourceId { get; set; }
    }
}