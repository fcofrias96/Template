using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Template.Core.Application.DTOs.Auth
{
    public class ResetPasswordDTO
    {
        [BindRequired]
        [EmailAddress]
        public string Email { get; set; }

        [BindRequired]
        public string Token { get; set; }

        [BindRequired]
        [MinLength(6)]
        public string Password { get; set; }

        [BindRequired]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}