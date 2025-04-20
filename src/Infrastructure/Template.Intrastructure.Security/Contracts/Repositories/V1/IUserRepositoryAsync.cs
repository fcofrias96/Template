using Template.Application.Contracts.Repositories.Base;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IUserRepositoryAsync : IReadRepositoryAsync<User, SecurityDbContext>
    {

        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);

        Task<IReadOnlyList<RoleListDto>> GetRoles(string userId);

        Task<UserDetailDto?> GetUserDetailAsync(string userId);

        Task UpdateUserRolesAsync(string userId, string[] newRoleIds);
    }
}