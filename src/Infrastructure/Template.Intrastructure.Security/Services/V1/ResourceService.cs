using AutoMapper;
using Microsoft.AspNetCore.Http;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services.Base;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class ResourceService : BaseServiceAsync<Resource>, IResourceServiceAsync
    {
        public ResourceService(ICacheService cache, IMapper mapper, IResourceRepositoryAsync repositoryAsync, IHttpContextAccessor httpContext) : base(cache, mapper, repositoryAsync, httpContext)
        {
        }
    }
}