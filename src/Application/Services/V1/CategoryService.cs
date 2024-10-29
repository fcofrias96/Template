using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Template.Application.Contracts.Repositories.V1;
using Template.Application.Contracts.Services.V1;
using Template.Domain.Entities.V1;

namespace Template.Application.Services.V1
{
    public class CategoryService : ServiceBaseAsync<Category>, ICategoryService
    {

        public CategoryService(
            ICategoryRepository repository,
            IMapper mapper,
            IHttpContextAccessor httpContext,
            IMemoryCache cache,
            ILogger logger
            ) : base(repository, mapper, httpContext, cache, logger)
        {
        }
    }
}