using System.ComponentModel.DataAnnotations;

namespace Template.Modules.Identity.Application.Dtos.Auth
{
    public class ConfirmEmailDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
