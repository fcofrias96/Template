using AutoMapper;
using Microsoft.AspNetCore.Http;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services.Base;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using PermissionAction = Template.Security.Entities.PermissionAction;

namespace Template.Security.Services.V1
{
    public class ActionService : BaseService<PermissionAction>, IActionService
    {
        public ActionService(BaseServiceDeps<PermissionAction> deps) : base(deps)
        {
        }
    }
}