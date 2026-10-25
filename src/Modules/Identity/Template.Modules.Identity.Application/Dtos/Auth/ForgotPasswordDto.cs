using System.ComponentModel.DataAnnotations;

namespace Template.Modules.Identity.Application.Dtos.Auth
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
