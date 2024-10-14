using Template.Application.Models.DTOs.V1.Permission;

namespace Template.Application.Models.DTOs.V1.Resource
{
    public class ResourcePermissionDto
    {
        public int Id { get; set; }
        public string Resource { get; set; }
        public List<PermissionListDto> Permissions { get; set; }
    }
}