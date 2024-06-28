using System.ComponentModel.DataAnnotations;

namespace Template.Api.Controllers
{
    public class ConfirmChangeEmailDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string NewEmail { get; set; }
        [Required]
        public Guid UserId { get; set; }

    }
}