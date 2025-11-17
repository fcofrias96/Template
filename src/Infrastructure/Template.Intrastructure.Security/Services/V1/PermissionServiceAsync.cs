using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Services.Base;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class PermissionServiceAsync : BaseServiceAsync<Permission, SecurityDbContext>, IPermissionServiceAsync
    {
        private readonly IPermissionRepositoryAsync _permissionRepository;

        public PermissionServiceAsync(
            IMemoryCache cache,
            IMapper mapper,
            IBaseRepositoryAsync<Permission, SecurityDbContext> repositoryAsync,
            IHttpContextAccessor httpContext, IPermissionRepositoryAsync permissionRepositoryAsync)
            : base(cache, mapper, repositoryAsync, httpContext)
        {
            _permissionRepository = permissionRepositoryAsync;
        }

        public async Task<bool> HasPermissionAsync(string userId, int resourceId, int actionId)
        {
            return await _permissionRepository.HasPermissionAsync(userId, resourceId, actionId);
        }
    }
}