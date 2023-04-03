using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Template.Core.Application.DTOs.Role;
using Template.Core.Application.Utils;
using Template.Infrastructure.Identity.Contracts.Repositories;
using Template.Infrastructure.Identity.Contracts.Services;

namespace Template.Infrastructure.Identity.Services
{
    public class RoleServiceAsync : IRoleServiceAsync
    {
        private readonly IRoleRepositoryAsync _roleRepositoryAsync;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;

        public RoleServiceAsync(IRoleRepositoryAsync roleRepositoryAsync, IMemoryCache cache, IMapper mapper)
        {
            _roleRepositoryAsync = roleRepositoryAsync;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<(IList<RoleListDTO>, string)> GetAllWithETagAsync()
        {
            string cacheKey = $"Role_List";

            if (!_cache.TryGetValue(cacheKey, out List<RoleListDTO> mappedRoles))
            {
                var entities = await _roleRepositoryAsync.GetAllAsync();
                mappedRoles = _mapper.Map<List<RoleListDTO>>(entities);
                _cache.Set(cacheKey, mappedRoles, TimeSpan.FromDays(7));
            }

            var eTag = CacheUtils.GenerateETag(mappedRoles);
            return (mappedRoles, eTag);
        }
    }
}