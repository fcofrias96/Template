using Template.Application.Contracts;

namespace Template.Application.Models.DTOs.V1.Permissions
{
    public class PermissionUpdateDto: IUpdateDto
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public int ResourceId { get; set; }
    }
}