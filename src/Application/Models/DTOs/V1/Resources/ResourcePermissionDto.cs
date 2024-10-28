using Template.Application.Models.DTOs.V1.Permissions;

namespace Template.Application.Models.DTOs.V1.Resources
{
    public class ResourcePermissionDto
    {
        public int Id { get; set; }
        public string Resource { get; set; }
        public List<PermissionListDto> Permissions { get; set; }
    }
}