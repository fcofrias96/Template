using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.User
{
    public class UserUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [StringLength(15, ErrorMessage = "Your password must be between {2} and {1} characters long.", MinimumLength = 6)]
        public string? CurrentPassword { get; set; }

        [StringLength(15, ErrorMessage = "Your password must be between {2} and {1} characters long.", MinimumLength = 6)]
        public string? NewPassword { get; set; }

        [Compare("NewPassword")]
        public string? ConfirmPassword { get; set; }
    }
}