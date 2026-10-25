using System.ComponentModel.DataAnnotations;

namespace Template.Modules.Identity.Application.Dtos.Roles
{
    public class RoleIdsDto
    {
        [Required]
        public int[] RoleIds { get; set; }
    }
}
