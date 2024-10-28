using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.Roles
{
    public class RoleIdsDTO
    {
        [Required]
        public List<Guid> RoleIds { get; set; }
    }
}