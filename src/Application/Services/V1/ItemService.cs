using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Repositories.V1;
using Template.Application.Contracts.Services.V1;
using Template.Domain.Entities.V1;

namespace Template.Application.Services.V1
{
    public class ItemService : ServiceBaseAsync<Item>, IItemService
    {
        public ItemService(IItemRepositoryAsync repository, IMapper mapper, IHttpContextAccessor httpContext, IMemoryCache cache, IItemRepositoryAsync itemRepository) : base(repository, mapper, httpContext, cache)
        {
        }
    }
}