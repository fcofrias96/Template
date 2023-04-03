using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Template.Core.Application.DTOs.Auth
{
    public class ForgotPasswordDTO
    {
        [BindRequired]
        [EmailAddress]
        public string Email { get; set; }
    }
}