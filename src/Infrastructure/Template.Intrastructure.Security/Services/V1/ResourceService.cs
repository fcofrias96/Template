using AutoMapper;
using Microsoft.AspNetCore.Http;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services.Base;
using Template.Domain.Entities.V1;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;

namespace Template.Security.Services.V1
{
    public class ResourceService : BaseService<Resource>, IResourceService
    {
        public ResourceService(ICacheService cache, IMapper mapper, IResourceRepository repositoryAsync, IHttpContextAccessor httpContext) : base(cache, mapper, repositoryAsync, httpContext)
        {
        }
    }
}