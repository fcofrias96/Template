using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Template.Core.Application.DTOs.User
{
    public class UserUpdateDTO
    {
        [BindRequired]
        public Guid Id { get; set; }

        [BindRequired]
        public string FirstName { get; set; }

        [BindRequired]
        public string LastName { get; set; }

        [StringLength(15, ErrorMessage = "Your password id limited to {2} to {1}", MinimumLength = 6)]
        public string? CurrentPassword { get; set; }

        [StringLength(15, ErrorMessage = "Your password id limited to {2} to {1}", MinimumLength = 6)]
        public string? NewPassword { get; set; }

        [Compare("NewPassword")]
        public string? ConfirmPassword { get; set; }
    }
}