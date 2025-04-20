using System.Text.Json.Serialization;
using Template.Application.Models.DTOs.V1.Permissions;

namespace Template.Application.Models.DTOs.V1.Resources
{
    public class ResourcePermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PermissionListDto[] Permissions { get; set; }
    }
}