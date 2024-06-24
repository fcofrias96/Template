using Template.Application.Models.DTOs.V1.Module;
using Template.Identity.Entities;

namespace Template.Identity.Contracts.Repositories.V1
{
    public interface IUserModulePermissionRepositoryAsync
    {
        Task<ModulePermissionDTO?> GetModulePermissionAsync(string userId, int moduleId);

        Task<List<ModulePermissionDTO>> GetModulePermissionsAsync(string userId);

        Task<bool> IsAuthorizedAsync(string userId, string module, string permission);

        Task DeleteAsync(UserModulePermission userModulePermission);

        Task CreateAsync(UserModulePermission userModulePermission);

        Task CreateRangeAsync(List<UserModulePermission> userModulePermissions);

        Task<List<UserModulePermission>> GetByUserIdAndModuleIdAsync(string userId, int moduleId);

        Task DeleteRangeAsync(List<UserModulePermission> userModulePermissions);

        Task<UserModulePermission?> GetAsync(string userId, int moduleId, int permissionId);

        Task<List<UserModulePermission>> GetAllAsync(string userId, int moduleId, List<int> permissionId);
    }
}