using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.Auth
{
    public class ConfirmEmailDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }
    }
}