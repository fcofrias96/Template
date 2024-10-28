using Template.Application.Models.DTOs.V1.Resources;

namespace Template.Application.Models.DTOs.V1.Users
{
    public class UserModulePermissionDTO
    {
        public List<ResourcePermissionDto> ModulePermissions { get; set; }
    }
}