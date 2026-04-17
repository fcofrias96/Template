using System;

namespace Template.Application.Models.DTOs.V1.Auth
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}