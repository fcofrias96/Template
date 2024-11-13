using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Models.DTOs.V1.Permissions;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class UserRepositoryAsync : IUserRepositoryAsync
    {
        private readonly SecurityDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IReadRepositoryAsync<User, SecurityDbContext> _readRepository;

        public UserRepositoryAsync(
            UserManager<User> userManager,
            SecurityDbContext dbContext,
            IReadRepositoryAsync<User, SecurityDbContext> readRepository
            )
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _readRepository = readRepository;
        }

        public async Task AddToRoleAsync(IdentityUserRole<string> userRoles)
        {
            //_dbContext.UserRoles.Add(userRoles);
            // await _dbContext.SaveChangesAsync();
        }

        public async Task AddToRolesAsync(List<IdentityUserRole<string>> identityUserRoles)
        {
            //await _dbContext.UserRoles.AddRangeAsync(identityUserRoles);
            //await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync<TKey>(TKey id)
        {
            return await _readRepository.GetByIdAsync(id);
        }

        public async Task<TDestination?> GetByIdProjectedAsync<TKey, TDestination>(TKey id)
        {
            return await _readRepository.GetByIdProjectedAsync<TKey, TDestination>(id);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>()
        {
            return await _readRepository.GetProjectedAsync<TDestination>();
        }

        public Task<IReadOnlyList<TDestination>> GetProjectedForExportAsync<TDestination>(ISpecification<User> spec)
        {
            return _readRepository.GetProjectedForExportAsync<TDestination>(spec);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedPaginatedAsync<TDestination>(ISpecification<User> spec)
        {
            return await _readRepository.GetProjectedPaginatedAsync<TDestination>(spec);
        }

        public async Task<IReadOnlyList<RoleListDto>> GetRoles(string userId)
        {
            return await _dbContext.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.UserRoles.Select(ur => new RoleListDto { Id = ur.RoleId, Name = ur.Role.Name }))
                .ToListAsync();
        }


        public async Task<UserDetailDto?> GetUserDetailAsync(string userId)
        {
            return await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserDetailDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName,
                    IsVerified = u.EmailConfirmed,
                    Roles = u.UserRoles
                        .Select(ur => new RoleListDto
                        {
                            Id = ur.RoleId,
                            Name = ur.Role.Name ?? string.Empty
                        })
                        .ToArray(),
                    Resources = u.UserRoles
                        .SelectMany(ur => ur.Role.RolePermissions)
                        .GroupBy(rp => rp.ResourceAction.Resource)
                        .Select(g => new ResourcePermissionDto
                        {
                            Id = g.Key.Id,
                            Resource = g.Key.Name,
                            Permissions = g.Select(rp => new PermissionListDto
                            {
                                Id = rp.ResourceActionId,
                                Name = rp.ResourceAction.Action.Name
                            }).ToArray()
                        })
                        .ToArray()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetTotalCountAsync(Expression<Func<User, bool>>? criteria)
        {
            return await _readRepository.GetTotalCountAsync(criteria);
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task RemoveFromRoleAsync(IdentityUserRole<string> userRole)
        {
            //_dbContext.UserRoles.Remove(userRole);
            //await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveFromRolesAsync(List<IdentityUserRole<string>> identityUserRoles)
        {
            //_dbContext.UserRoles.RemoveRange(identityUserRoles);
            //await _dbContext.SaveChangesAsync();
        }
    }
}