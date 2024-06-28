using System.ComponentModel.DataAnnotations;

namespace Template.Api.Controllers
{
    public class ChangeEmailDto
    {
        [Required]
        public string NewEmail { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string CurrentPassword { get; set; }

    }
}