using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Template.Core.Application.DTOs.User
{
    public class UserCreateDTO
    {
        [BindRequired]
        public string FirstName { get; set; }

        [BindRequired]
        public string LastName { get; set; }

        [BindRequired]
        public string UserName { get; set; }

        [BindRequired]
        [EmailAddress]
        public string Email { get; set; }

        [BindRequired]
        [StringLength(15, ErrorMessage = "Your password id limited to {2} to {1}", MinimumLength = 6)]
        public string Password { get; set; }

        [BindRequired]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}