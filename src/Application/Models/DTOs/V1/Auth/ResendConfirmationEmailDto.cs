using System.ComponentModel.DataAnnotations;

namespace Template.Identity.Services.V1
{
    public class ResendConfirmationEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}