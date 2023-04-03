using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Template.Core.Application.Contracts.Repositories;
using Template.Core.Application.Contracts.Services;
using Template.Core.Domain.Entities;

namespace Template.Core.Application.Services
{
    public class ItemServiceAsync : GenericServiceAsync<Item>, IItemServiceAsync
    {
        #region Constructor

        public ItemServiceAsync(IItemRepositoryAsync repository, IMapper mapper, IHttpContextAccessor httpContext, IMemoryCache cache, IItemRepositoryAsync itemRepository) : base(repository, mapper, httpContext, cache)
        {
        }

        #endregion Constructor
    }
}