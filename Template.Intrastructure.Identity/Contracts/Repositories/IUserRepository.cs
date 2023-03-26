using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Template.Core.Application.DTOs.User;
using Template.Core.Domain.Entities;
using Template.Infrastructure.Identity.Entity;

namespace Template.Infrastructure.Identity.Contracts.Repositories
{
    public interface IUserRepositoryAsync
    {
        Task AddToRoleAsync(User user, string role);

        Task<IdentityResult> AddToRolesAsync(User user, List<string> roles);

        Task<IdentityResult> ChangeEmailAsync(User user, string currentPassword, string newPassword);

        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

        Task<IdentityResult> CreateAsync(User user, string password);
        Task DeleteAsync(User entity);

        Task<User> FindByEmailAsync(string email);

        Task<User> FindByIdAsync(string userId);

        Task<User> FindByNameAsync(string userName);

        Task<List<UserListDTO>> GetAllAsync(QueryParameters<User> queryParameters);

        Task<IList<Claim>> GetClaimsAsync(User user);

        Task<IList<string>> GetRolesAsync(User user);

        Task<int> GetTotalRecordsAsync();

        Task<IdentityResult> RemoveFromRolesAsync(User user, List<string> roles);
        Task UpdateAsync(User user);

        Task AddClaimAsync(User user, Claim claims);
    }
}
