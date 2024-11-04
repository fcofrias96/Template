using Microsoft.AspNetCore.Identity;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;

namespace Template.Identity.Contracts.Repositories.V1
{
    public interface IUserRepositoryAsync: IReadRepositoryAsync<User, IdentityContext>
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