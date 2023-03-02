using System.ComponentModel.DataAnnotations;

namespace Template.Core.Application.DTOs.Auth
{
    public class AuthenticationRequest
    {
        [Required(ErrorMessage = "el nombre de usuario es necesario")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "la contraseña es nesecaria")]
        public string Password { get; set; }
    }
}