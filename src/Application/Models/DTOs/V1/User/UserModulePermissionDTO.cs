using Template.Application.Models.DTOs.V1.Resource;

namespace Template.Application.Models.DTOs.V1.User
{
    public class UserModulePermissionDTO
    {
        public List<ResourcePermissionDto> ModulePermissions { get; set; }
    }
}