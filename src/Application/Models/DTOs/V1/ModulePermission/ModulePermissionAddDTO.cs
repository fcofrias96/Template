using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.ModulePermission
{
    public class ModulePermissionAddDTO
    {
        [Range(1, int.MaxValue)]
        public int ModuleId { get; set; }

        [Required]
        public List<int> PermissionIds { get; set; }
    }
}