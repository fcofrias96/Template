using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.Roles
{
    public class RoleIdsDto
    {
        [Required]
        public int[] RoleIds { get; set; }
    }
}