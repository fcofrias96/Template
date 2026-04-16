using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.Auth
{
    public class ResetPasswordDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Your password must be between {2} and {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}