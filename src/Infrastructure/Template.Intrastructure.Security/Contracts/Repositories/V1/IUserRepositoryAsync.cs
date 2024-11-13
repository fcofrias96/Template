using Microsoft.AspNetCore.Identity;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IUserRepositoryAsync : IReadRepositoryAsync<User, SecurityDbContext>
    {
        Task AddToRoleAsync(IdentityUserRole<string> userRoles);

        Task AddToRolesAsync(List<IdentityUserRole<string>> identityUserRoles);

        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);

        Task RemoveFromRoleAsync(IdentityUserRole<string> userRoles);

        Task RemoveFromRolesAsync(List<IdentityUserRole<string>> identityUserRoles);

        Task<IReadOnlyList<RoleListDto>> GetRoles(string userId);

        Task<UserDetailDto?> GetUserDetailAsync(string userId);
    }
}