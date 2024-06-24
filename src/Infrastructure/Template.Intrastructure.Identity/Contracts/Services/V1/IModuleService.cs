using Template.Application.Models.DTOs.V1.Module;
using Template.Application.Models.Wrappers;
using Template.Identity.Entities;

namespace Template.Identity.Contracts.Services.V1
{
    public interface IModuleService
    {
        Task<Module> CreateAsync(ModuleCreateDTO moduleCreate);

        Task DeletePermanentAsync(int id);

        Task<ApiResponse<List<ModuleListDTO>>> GetAllProjectedAsync();

        Task<ApiResponse<ModuleDetailDTO>> GetByIdProjectedAsync(int id);

        Task UpdateAsync(int id, ModuleUpdateDTO source);

        Task<ApiResponse<ModulePermissionDTO>> GetModulePermissionsAsync(string UserId, int moduleId);
    }
}