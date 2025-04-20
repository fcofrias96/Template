using Template.Application.Models.DTOs.V1.Permissions;
using Template.Application.Models.DTOs.V1.Resources;

namespace Template.Application.Models.DTOs.V1.Roles
{
    public class RoleDetailDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public ResourcePermissionDto[] Resources { get; set; }

        public PermissionListDto[] Permissions { get; set; }
    }
}