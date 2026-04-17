using Template.Application.Services.Base;
using Template.Security.Contracts.Services.V1;
using Template.Security.Entities;


namespace Template.Security.Services.V1
{
    public class ActionService : BaseService<PermissionAction>, IActionService
    {
        public ActionService(BaseServiceDeps<PermissionAction> deps) : base(deps)
        {
        }
    }
}