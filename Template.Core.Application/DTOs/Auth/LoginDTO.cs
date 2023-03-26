using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Template.Core.Application.DTOs.Auth
{
    public class LoginDTO
    {
        [BindRequired]
        public string UserName { get; set; }

        [BindRequired]
        public string Password { get; set; }
    }
}