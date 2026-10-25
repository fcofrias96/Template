using Template.Modules.Identity.Application.Dtos.Permissions;

namespace Template.Modules.Identity.Application.Dtos.Resources
{
    public class ResourcePermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PermissionListDto[] Permissions { get; set; }
    }
}
