using Template.Application.Models.DTOs.V1.Permission;

namespace Template.Application.Models.DTOs.V1.Module
{
    public class ModulePermissionDTO
    {
        public int Id { get; set; }
        public string Module { get; set; }
        public List<PermissionListDTO> Permissions { get; set; }
    }
}