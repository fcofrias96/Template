using System.ComponentModel.DataAnnotations;

namespace Template.Core.Application.DTOs.User
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Your password id limited to {2} to {1}", MinimumLength = 6)]
        public string Password { get; set; }
    }
}