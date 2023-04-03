using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Template.Core.Application.DTOs.User;
using Template.Core.Domain.Entities;
using Template.Infrastructure.Identity.Contracts.Repositories;
using Template.Infrastructure.Identity.Entity;

namespace Template.Infrastructure.Identity.Repositories
{
    public class UserRepositoryAsync : IUserRepositoryAsync
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UserRepositoryAsync(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task AddToRoleAsync(User user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> AddToRolesAsync(User user, List<string> roles)
        {
            return await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task<IdentityResult> ChangeEmailAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangeEmailAsync(user, currentPassword, newPassword);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public Task<IdentityResult> CreateAsync(User user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public async Task DeleteAsync(User entity)
        {
            await _userManager.DeleteAsync(entity);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User> FindByIdAsync(string Id)
        {
            return await _userManager.FindByIdAsync(Id);
        }

        public async Task<User> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public Task<List<TSource>> GetAllAsync<TSource>(QueryParameters<User> queryParameters, List<string>? navigationPropertyToInclude = null)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserListDTO>> GetAllAsync(QueryParameters<User> queryParameters)
        {
            return await _userManager.Users
                        .Skip(queryParameters.Offset)
                        .Take(queryParameters.Limit)
                        .ProjectTo<UserListDTO>(_mapper.ConfigurationProvider)
                        .ToListAsync();
        }

        public async Task<IList<Claim>> GetClaimsAsync(User user)
        {
            return await _userManager.GetClaimsAsync(user);
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<int> GetTotalRecordsAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<IdentityResult> RemoveFromRolesAsync(User user, List<string> roles)
        {
            return await _userManager.RemoveFromRolesAsync(user, roles);
        }

        public async Task UpdateAsync(User user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task AddClaimAsync(User user, Claim claims)
        {
            await _userManager.AddClaimAsync(user, claims);
        }
    }
}