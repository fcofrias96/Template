using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Template.Application.Models.Wrappers;
using Template.Application.Services;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Entities;

namespace Template.Identity.Services.V1
{
    public class RoleServiceAsync : ServiceBaseAsync<Role>, IRoleServiceAsync
    {
        public RoleServiceAsync(
            IRoleRepositoryAsync repository,
            IMapper mapper, 
            IHttpContextAccessor httpContext, 
            IMemoryCache cache,
            ILogger logger

            ) : base(repository, mapper, httpContext, cache, logger)
        {
        }


        public override Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id)
        {
            string idString = id?.ToString() ?? string.Empty;
            return base.GetByIdProjectedAsync<string, TDestination>(idString);
        }
    }
}