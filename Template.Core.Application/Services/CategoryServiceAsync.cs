using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Template.Core.Application.Contracts.Repositories;
using Template.Core.Application.Contracts.Services;
using Template.Core.Domain.Entities;

namespace Template.Core.Application.Services
{
    public class CategoryServiceAsync : GenericServiceAsync<Category>, ICategoryServiceAsync
    {
        #region Constructor

        public CategoryServiceAsync(ICategoryRepositoryAsync repository, IMapper mapper, IHttpContextAccessor httpContext, IMemoryCache cache) : base(repository, mapper, httpContext, cache)
        {
        }

        #endregion Constructor
    }
}