using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.Modules.Identity.Application.Dtos.Permissions;
using Template.Modules.Identity.Application.Dtos.Resources;
using Template.Modules.Identity.Application.Dtos.Roles;
using Template.Modules.Identity.Application.Dtos.Users;
using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Infrastructure;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        private readonly IdentityModuleDbContext _dbContext;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            IdentityModuleDbContext dbContext
            ): base(dbContext)
        {
            _dbContext = dbContext;
        }

      
        public async Task<IReadOnlyList<RoleListDto>> GetRolesAsync(int userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.UserRoles.Select(ur => new RoleListDto { Id = ur.RoleId, Name = ur.Role.Name ?? String.Empty }))
                .ToListAsync(cancellationToken);
        }

        public async Task<UserDetailDto?> GetDetailAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
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
                        .ToArray()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is not null)
            {
                user.Resources = await GetResourcesAsync(userId, cancellationToken);
            }

            return user;
        }


        public async Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
        }

        public async Task<ResourcePermissionDto[]> GetResourcesAsync(int userId, CancellationToken cancellationToken)
        {
            // Flat query + in-memory grouping: correlated GroupBy is not translatable by every provider (e.g. InMemory).
            var rows = await _dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => new
                {
                    ResourceId = rp.Permission.Resource.Id,
                    ResourceName = rp.Permission.Resource.Name,
                    rp.PermissionId,
                    ActionName = rp.Permission.Action.Name
                })
                .ToListAsync(cancellationToken);

            return rows
                .GroupBy(row => new { row.ResourceId, row.ResourceName })
                .Select(g => new ResourcePermissionDto
                {
                    Id = g.Key.ResourceId,
                    Name = g.Key.ResourceName,
                    Permissions = g
                        .DistinctBy(row => row.PermissionId)
                        .Select(row => new PermissionListDto
                        {
                            Id = row.PermissionId,
                            Name = row.ActionName
                        })
                        .ToArray()
                })
                .ToArray();
        }

        public async Task<int?> GetIdByPublicIdAsync(Guid publicId, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.PublicId == publicId && !u.IsDeleted)
                .Select(u => (int?)u.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
