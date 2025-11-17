using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Services.Base;
using Template.Domain.Entities.V1;
using Template.Persistence.Contracts.Services;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Services.V1
{
    public class ItemService : BaseServiceAsync<Item, ApplicationDbContext>, IItemService
    {
        public ItemService(IMemoryCache cache, IMapper mapper, IBaseRepositoryAsync<Item, ApplicationDbContext> repositoryAsync, IHttpContextAccessor httpContext) : base(cache, mapper, repositoryAsync, httpContext)
        {
        }   
    }
}