using Template.BuildingBlocks.Application.Contracts.Services.Base;
using PermissionAction = Template.Modules.Identity.Domain.Entities.PermissionAction;

namespace Template.Modules.Identity.Application.Contracts.Services
{
    public interface IActionService : IReadService<PermissionAction>
    {
    }
}
