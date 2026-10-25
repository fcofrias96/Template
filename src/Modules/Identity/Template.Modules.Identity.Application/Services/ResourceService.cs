using Microsoft.AspNetCore.Http;
using Template.BuildingBlocks.Application.Caching;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.BuildingBlocks.Application.Services.Base;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Services
{
    public class ResourceService : BaseService<Resource>, IResourceService
    {
        public ResourceService(
            IResourceRepository repository,
            ICacheService cache,
            IHttpContextAccessor httpContext)
            : base(repository, cache, httpContext)
        {
        }
    }
}
