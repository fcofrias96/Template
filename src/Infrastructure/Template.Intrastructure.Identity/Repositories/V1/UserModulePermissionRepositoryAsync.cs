using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Template.Application.Constants;
using Template.Application.Models.DTOs.V1.Module;
using Template.Application.Models.DTOs.V1.Permission;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.DbContext.V1;
using Template.Identity.Entities;

namespace Template.Identity.Repositories.V1
{
    public class UserModulePermissionRepositoryAsync : IUserModulePermissionRepositoryAsync
    {
        private readonly IMapper _mapper;
        private readonly IdentityContext _dbContext;

        public UserModulePermissionRepositoryAsync(IMapper mapper, IdentityContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task CreateAsync(UserModulePermission userModulePermission)
        {
            await _dbContext.AddAsync(userModulePermission);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserModulePermission userModulePermission)
        {
            _dbContext.Remove(userModulePermission);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ModulePermissionDTO>> GetModulePermissionsAsync(string userId)
        {
            return await _dbContext.UserModulePermissions.Where(u => u.UserId == userId)
            .GroupBy(ump => ump.ModuleId)
            .Select(group => new ModulePermissionDTO
            {
                Id = group.Key,
                Module = group.First().Module.Name,
                Permissions = group.Select(ump => new PermissionListDTO { Id = ump.PermissionId, Name = ump.Permission.Name }).ToList()
            }).ToListAsync();
        }

        public async Task<ModulePermissionDTO?> GetModulePermissionAsync(string userId, int moduleId)
        {
            return await _dbContext.UserModulePermissions.Where(u => u.UserId == userId && u.ModuleId == moduleId)
            .GroupBy(ump => ump.ModuleId)
            .Select(group => new ModulePermissionDTO
            {
                Id = group.Key,
                Module = group.First().Module.Name,
                Permissions = group.Select(ump => new PermissionListDTO { Id = ump.PermissionId, Name = ump.Permission.Name }).ToList()
            }).FirstOrDefaultAsync();
        }

        public async Task<bool> IsAuthorizedAsync(string userId, string module, string permission)
        {
            return await _dbContext.UserModulePermissions
                                   .AnyAsync(ump => ump.UserId == userId.ToString()
                                                    && ump.Module.Name == module
                                                    && (ump.Permission.Name == permission
                                                        || ump.Permission.Name == Permissions.All));
        }

        public async Task CreateRangeAsync(List<UserModulePermission> userModulePermissions)
        {
            await _dbContext.AddRangeAsync(userModulePermissions);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<UserModulePermission>> GetByUserIdAndModuleIdAsync(string userId, int moduleId)
        {
            return await _dbContext.UserModulePermissions
                     .Where(ump => ump.UserId == userId && ump.ModuleId == moduleId)
                     .ToListAsync();
        }

        public async Task DeleteRangeAsync(List<UserModulePermission> userModulePermissions)
        {
            _dbContext.RemoveRange(userModulePermissions);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserModulePermission?> GetAsync(string userId, int moduleId, int permissionId)
        {
            return await _dbContext.UserModulePermissions
                            .Where(ump => ump.UserId == userId && ump.ModuleId == moduleId && ump.PermissionId == permissionId)
                            .FirstOrDefaultAsync();
        }

        public async Task<List<UserModulePermission>> GetAllAsync(string userId, int moduleId, List<int> permissionId)
        {
            return await _dbContext.UserModulePermissions
                            .Where(ump => ump.UserId == userId && ump.ModuleId == moduleId && permissionId.Contains(ump.PermissionId))
                            .ToListAsync();
        }
    }
}