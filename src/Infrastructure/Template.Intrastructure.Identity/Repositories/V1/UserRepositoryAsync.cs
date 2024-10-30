using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Template.Application.Models.DTOs.V1.Users;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;
using Template.Identity.Specification.V1;

namespace Template.Identity.Repositories.V1
{
    public class UserRepositoryAsync : IUserRepositoryAsync
    {
        private readonly IdentityContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UserRepositoryAsync(UserManager<User> userManager, IMapper mapper, IdentityContext dbContext)
        {
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public Task AddToRoleAsync(IdentityUserRole<string> userRoles)
        {
            _dbContext.UserRoles.Add(userRoles);
            return _dbContext.SaveChangesAsync();
        }

        public async Task AddToRolesAsync(List<IdentityUserRole<string>> identityUserRoles)
        {
            await _dbContext.UserRoles.AddRangeAsync(identityUserRoles);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<UserListDTO>> GetAllProjectedWithPaginationAsync(UserSpecification spec)
        {
            var query = _userManager.Users.AsQueryable();

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            if (spec.Descending)
            {
                query = query.OrderByDescending(spec.SortBy);
            }
            else
            {
                query = query.OrderBy(spec.SortBy);
            }

            return await query.Skip(spec.Skip)
                              .Take(spec.Take)
                              .ProjectTo<UserListDTO>(_mapper.ConfigurationProvider)
                              .ToListAsync();
        }

        public async Task<int> GetTotalRecordsAsync(Expression<Func<User, bool>>? criteria)
        {
            if (criteria == null)
            {
                return await _dbContext.Users.CountAsync();
            }
            else
            {
                return await _dbContext.Users.Where(criteria).CountAsync();
            }
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task RemoveFromRoleAsync(IdentityUserRole<string> userRole)
        {
            _dbContext.UserRoles.Remove(userRole);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveFromRolesAsync(List<IdentityUserRole<string>> identityUserRoles)
        {
            _dbContext.UserRoles.RemoveRange(identityUserRoles);
            await _dbContext.SaveChangesAsync();
        }
    }
}