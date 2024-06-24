using Microsoft.Extensions.Caching.Memory;
using Template.Application.Models.DTOs.V1.Role;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Contracts.Services.V1;

namespace Template.Identity.Services.V1
{
    public class RoleServiceAsync : IRoleServiceAsync
    {
        private readonly IRoleRepositoryAsync _roleRepositoryAsync;
        private readonly IMemoryCache _cache;

        public RoleServiceAsync(IRoleRepositoryAsync roleRepositoryAsync, IMemoryCache cache)
        {
            _roleRepositoryAsync = roleRepositoryAsync;
            _cache = cache;
        }

        public async Task<ApiResponse<List<RoleListDTO>>> GetAllAsync()
        {
            string cacheKey = $"RolesCache";

            if (!_cache.TryGetValue(cacheKey, out List<RoleListDTO> roles))
            {
                roles = await _roleRepositoryAsync.GetAllAsync();
                _cache.Set(cacheKey, roles);
            }

            return new ApiResponse<List<RoleListDTO>>(roles);
        }
    }
}