using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services.Base;
using Template.Domain.Entities.V1;
using Template.Persistence.Contracts.Repositories.V1;
using Template.Persistence.Contracts.Services;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Services.V1
{
    public class CategoryService : BaseServiceAsync<Category>, ICategoryService
    {
        public CategoryService(ICacheService cache, IMapper mapper, ICategoryRepository repositoryAsync, IHttpContextAccessor httpContext) : base(cache, mapper, repositoryAsync, httpContext)
        {
        }
    }
}