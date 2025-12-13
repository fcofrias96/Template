using AutoMapper;
using Microsoft.AspNetCore.Http;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services.Base;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using ActionType = Template.Security.Entities.ActionType;

namespace Template.Security.Services.V1
{
    public class ActionService : BaseService<ActionType>, IActionService
    {
        public ActionService(ICacheService cache, IMapper mapper, IActionRepository  repositoryAsync, IHttpContextAccessor httpContext) : base(cache, mapper, repositoryAsync, httpContext)
        {
        }
    }
}