using AutoMapper;
using Template.Core.Application.Contracts.Repositories;
using Template.Core.Domain.Entities;
using Template.Core.Application.Contracts.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Template.Core.Application.Services
{
    public class ItemServiceAsync : GenericServiceAsync<Item>, IItemServiceAsync
    {
        #region Constructor
        public ItemServiceAsync(IItemRepositoryAsync repository, IMapper mapper, IHttpContextAccessor httpContext, IMemoryCache cache) : base(repository, mapper, httpContext, cache)
        {
        }
        #endregion Constructor
    }
}
