using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Services;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Contracts.Services.V1;
using Action = Template.Identity.Entities.Action;

namespace Template.Identity.Services.V1
{
    public class ActionServiceAsync : ServiceBaseAsync<Action>, IActionServiceAsync
    {
        public ActionServiceAsync(IActionRepositoryAsync repository, IMapper mapper, IHttpContextAccessor httpContext, IMemoryCache cache) : base(repository, mapper, httpContext, cache)
        {
        }
    }
}