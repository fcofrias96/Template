using Template.Application.Contracts.Services.Base;
using PermissionAction = Template.Security.Entities.PermissionAction;

namespace Template.Security.Contracts.Services.V1
{
    public interface IActionService : IReadService<PermissionAction>
    {
    }
}