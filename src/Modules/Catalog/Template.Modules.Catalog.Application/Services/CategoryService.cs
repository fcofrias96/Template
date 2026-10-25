using Microsoft.AspNetCore.Http;
using Template.BuildingBlocks.Application.Caching;
using Template.Modules.Catalog.Application.Contracts.Repositories;
using Template.BuildingBlocks.Application.Services.Base;
using Template.Modules.Catalog.Domain.Entities;
using Template.Modules.Catalog.Application.Contracts.Services;

namespace Template.Modules.Catalog.Application.Services
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        public CategoryService(
            ICategoryRepository repository,
            ICacheService cache,
            IHttpContextAccessor httpContext)
            : base(repository, cache, httpContext)
        {
        }
    }
}
