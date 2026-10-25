using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Users
{
    public class UserListDto : IProjection<ApplicationUser, UserListDto>
    {
        public static Expression<Func<ApplicationUser, UserListDto>> Projection => user => new UserListDto
        {
            Id = user.Id.ToString(),
            UserName = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            Roles = user.UserRoles.Select(ur => ur.Role.Name!).ToArray()
        };

        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
    }
}
