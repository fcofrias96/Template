using System.ComponentModel.DataAnnotations;

namespace Template.Core.Application.DTOs.User
{
    public class ApiUserDTO : LoginDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}