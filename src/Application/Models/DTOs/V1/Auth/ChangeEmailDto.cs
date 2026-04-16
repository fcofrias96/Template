using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.Auth
{
    public class ChangeEmailDto
    {
        [Required]
        public string NewEmail { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string CurrentPassword { get; set; }
    }
}