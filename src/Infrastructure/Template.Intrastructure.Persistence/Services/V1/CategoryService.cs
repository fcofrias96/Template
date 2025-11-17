using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Services.Base;
using Template.Domain.Entities.V1;
using Template.Persistence.Contracts.Services;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Services.V1
{
    public class CategoryService : BaseServiceAsync<Category, ApplicationDbContext>, ICategoryService
    {
        public CategoryService(IMemoryCache cache, IMapper mapper, IBaseRepositoryAsync<Category, ApplicationDbContext> repositoryAsync, IHttpContextAccessor httpContext) : base(cache, mapper, repositoryAsync, httpContext)
        {
        }
    }
}