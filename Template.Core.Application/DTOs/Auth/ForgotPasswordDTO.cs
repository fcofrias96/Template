using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Template.Core.Application.DTOs.Auth
{
    public class ForgotPasswordDTO
    {
        [BindRequired]
        [EmailAddress]
        public string Email { get; set; }
    }
}
