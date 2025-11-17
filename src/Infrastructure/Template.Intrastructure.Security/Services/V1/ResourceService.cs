using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Services.Base;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class ResourceService : BaseServiceAsync<Resource, SecurityDbContext>, IResourceServiceAsync
    {
        public ResourceService(IMemoryCache cache, IMapper mapper, IBaseRepositoryAsync<Resource, SecurityDbContext> repositoryAsync, IHttpContextAccessor httpContext) : base(cache, mapper, repositoryAsync, httpContext)
        {
        }
    }
}