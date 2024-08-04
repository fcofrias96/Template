using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.Auth
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