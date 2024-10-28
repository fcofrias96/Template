using System.ComponentModel.DataAnnotations;

namespace Template.Application.Models.DTOs.V1.Users
{
    public class UserUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

    }
}