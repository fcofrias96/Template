using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Repositories.V1;
using Template.Application.Contracts.Services.V1;
using Template.Domain.Entities.V1;

namespace Template.Application.Services.V1
{
    public class ItemService : ServiceBase<Item>, IItemService
    {
        public ItemService(IItemRepository repository, IMapper mapper, IHttpContextAccessor httpContext, IMemoryCache cache, IItemRepository itemRepository) : base(repository, mapper, httpContext, cache)
        {
        }
    }
}