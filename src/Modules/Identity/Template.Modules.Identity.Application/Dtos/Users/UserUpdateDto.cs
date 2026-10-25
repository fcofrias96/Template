using System.ComponentModel.DataAnnotations;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Users
{
    public class UserUpdateDto : IUpdateMapTo<ApplicationUser>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public void ApplyTo(ApplicationUser entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            entity.FirstName = FirstName;
            entity.LastName = LastName;
            entity.UserName = UserName;
            entity.Email = Email;
        }
    }
}
