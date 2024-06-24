using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.ModulePermission
{
    public class ModulePermissionIdsDTO
    {
        [Required]
        public List<int> PermissionIds { get; set; }
    }
}