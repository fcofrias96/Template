using Template.Application.Contracts.Services.Base;
using PermissionAction = Template.Domain.Entities.V1.PermissionAction;

namespace Template.Security.Contracts.Services.V1
{
    public interface IActionService : IReadService<PermissionAction>
    {
    }
}