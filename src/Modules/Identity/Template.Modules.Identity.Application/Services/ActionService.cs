using Microsoft.AspNetCore.Http;
using Template.BuildingBlocks.Application.Caching;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.BuildingBlocks.Application.Services.Base;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Domain.Entities;


namespace Template.Modules.Identity.Application.Services
{
    public class ActionService : BaseService<PermissionAction>, IActionService
    {
        public ActionService(
            IActionRepository repository,
            ICacheService cache,
            IHttpContextAccessor httpContext)
            : base(repository, cache, httpContext)
        {
        }
    }
}
