using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.Auth
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}