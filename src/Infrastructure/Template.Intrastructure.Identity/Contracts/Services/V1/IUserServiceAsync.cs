using Template.Application.Models.DTOs.V1.Module;
using Template.Application.Models.DTOs.V1.ModulePermission;
using Template.Application.Models.DTOs.V1.User;
using Template.Application.Models.Wrappers;
using Template.Identity.Specification.V1;

namespace Template.Identity.Contracts.Services.V1
{
    public interface IUserServiceAsync
    {
        Task AddModulesAsync(Guid userId, List<ModulePermissionAddDTO> modulePermissions);

        Task AddPermissionsFromModuleAsync(Guid userId, int moduleId, List<int> permissionIds);

        Task AddToRolesAsync(Guid userId, List<Guid> roles);

        Task AddToRolesAsync(Guid userId, Guid roleId);

        Task DeleteAsync(Guid userId);

        Task<ApiResponse<PagedCollection<UserListDTO>>> GetAllAsync(UserFilterRequest request);

        Task<ApiResponse<UserDetailDTO>> GetByIdAsync(Guid userId);

        Task<ApiResponse<ModulePermissionDTO>> GetModuleAsync(Guid userId, int moduleId);

        Task<ApiResponse<List<ModulePermissionDTO>>> GetModulesAsync(Guid userId);

        Task<ApiResponse<List<string>>> GetRolesAsync(Guid userId);

        Task<bool> IsAuthorized(string userId, string module, string permission);

        Task RemoveFromRolesAsync(Guid userId, List<Guid> roles);

        Task RemoveFromRolesAsync(Guid userId, Guid roles);

        Task RemoveModuleAsync(Guid userId, int moduleId);

        Task RemovePermissionsFromModuleAsync(Guid userId, int moduleId, List<int> permissionIds);

        Task UpdateAsync(Guid userId, UserUpdateDTO userUpdateDTO);
    }
}