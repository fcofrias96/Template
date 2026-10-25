using System.ComponentModel.DataAnnotations;

namespace Template.Modules.Identity.Application.Dtos.Auth
{
    public class ResendConfirmationEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
