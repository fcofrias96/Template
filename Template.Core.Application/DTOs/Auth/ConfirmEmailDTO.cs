using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Template.Core.Application.DTOs.Auth
{
    public class ConfirmEmailDTO
    {
        [BindRequired]
        public string UserId { get; set; }
        [BindRequired]
        public string Code { get; set; }
    }
}
