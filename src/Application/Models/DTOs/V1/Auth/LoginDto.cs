using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.Auth
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}