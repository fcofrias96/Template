using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Application.Models.DTOs.V1.Module;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Entities;

namespace Template.Identity.Services.V1
{
    public class ModuleService : IModuleService
    {
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly IModuleRepository _repository;
        private readonly IUserModulePermissionRepositoryAsync _userModulePermissionRepositoryAsync;

        public ModuleService(IModuleRepository repository, IMapper mapper, IMemoryCache cache, IUserModulePermissionRepositoryAsync userModulePermissionRepositoryAsync)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
            _userModulePermissionRepositoryAsync = userModulePermissionRepositoryAsync;
        }

        public async Task<Module> CreateAsync(ModuleCreateDTO moduleCreate)
        {
            var module = _mapper.Map<Module>(moduleCreate);
            await _repository.CreateAsync(module);
            _cache.InvalidateCache<Module>();
            return module;
        }

        public async Task DeletePermanentAsync(int id)
        {
            var module = await GetByIdAsync(id);
            await _repository.DeleteAsync(module);
            _cache.InvalidateCache<Module>();
        }

        public async Task<ApiResponse<List<ModuleListDTO>>> GetAllProjectedAsync()
        {
            string cacheKey = $"{typeof(Module).FullName}Cache";

            if (!_cache.TryGetValue(cacheKey, out List<ModuleListDTO> mappedEntities))
            {
                mappedEntities = await _repository.GetAllProjectedAsync();

                _cache.Set(cacheKey, mappedEntities, TimeSpan.FromHours(24));
            }

            return new ApiResponse<List<ModuleListDTO>>(mappedEntities);
        }

        public async Task<ApiResponse<ModuleDetailDTO>> GetByIdProjectedAsync(int id)
        {
            var mappedEntity = await _repository.GetByIdProjectedAsync(id);
            if (mappedEntity is null)
            {
                throw new NotFoundException(typeof(Module).Name, id);
            }

            return new ApiResponse<ModuleDetailDTO>(mappedEntity);
        }

        public async Task<ApiResponse<ModulePermissionDTO?>> GetModulePermissionsAsync(string UserId, int moduleId)
        {
            var modulePermission = await _userModulePermissionRepositoryAsync.GetModulePermissionAsync(UserId, moduleId);

            return new ApiResponse<ModulePermissionDTO?>(modulePermission);
        }

        public async Task UpdateAsync(int id, ModuleUpdateDTO source)
        {
            var module = await GetByIdAsync(id);
            _mapper.Map(source, module);
            _cache.InvalidateCache<Module>();
            await _repository.UpdateAsync(module);
        }

        private async Task<Module> GetByIdAsync(int id)
        {
            var module = await _repository.GetByIdAsync(id);

            if (module is null)
            {
                throw new NotFoundException(typeof(Module).Name, id);
            }

            return module;
        }
    }
}