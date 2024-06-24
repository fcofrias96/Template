using Template.Application.Models.DTOs.V1.Module;
using Template.Identity.Entities;

namespace Template.Identity.Contracts.Repositories.V1
{
    public interface IModuleRepository
    {
        Task CreateAsync(Module entity);

        Task DeleteAsync(Module entity);

        Task<List<ModuleListDTO>> GetAllProjectedAsync();

        Task<Module?> GetByIdAsync(int id);

        Task<ModuleDetailDTO?> GetByIdProjectedAsync(int id);

        Task UpdateAsync(Module entity);
    }
}