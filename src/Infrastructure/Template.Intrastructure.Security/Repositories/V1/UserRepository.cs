using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.Application.Models.DTOs.V1.Permissions;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        private readonly SecurityDbContext _dbContext;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            SecurityDbContext dbContext,
            IConfigurationProvider configurationProvider
            ): base(dbContext, configurationProvider)
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
            return await _dbContext.Users
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
                        .ToArray(),
                    Resources = u.UserRoles
                        .SelectMany(ur => ur.Role.RolePermissions)
                        .GroupBy(rp => rp.Permission.Resource)
                        .Select(g => new ResourcePermissionDto
                        {
                            Id = g.Key.Id,
                            Name = g.Key.Name,
                            Permissions = g.Select(rp => new PermissionListDto
                            {
                                Id = rp.PermissionId,
                                Name = rp.Permission.Action.Name
                            }).ToArray()
                        })
                        .ToArray()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
        }

        public async Task<ResourcePermissionDto[]> GetResourcesAsync(int userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .GroupBy(rp => rp.Permission.Resource)
                    .Select(g => new ResourcePermissionDto
                    {
                        Id = g.Key.Id,
                        Name = g.Key.Name,
                        Permissions = g.Select(rp => new PermissionListDto
                        {
                            Id = rp.PermissionId,
                            Name = rp.Permission.Action.Name
                        }).ToArray()
                    }))
                .ToArrayAsync(cancellationToken);
        }
    }
}