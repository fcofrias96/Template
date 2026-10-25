using System.ComponentModel.DataAnnotations;

namespace Template.Modules.Identity.Application.Dtos.Auth
{
    public class ConfirmChangeEmailDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string NewEmail { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
