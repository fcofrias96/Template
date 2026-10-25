using Template.BuildingBlocks.Application.Contracts.Repositories.Base;
using PermissionAction = Template.Modules.Identity.Domain.Entities.PermissionAction;

namespace Template.Modules.Identity.Application.Contracts.Repositories
{
    public interface IActionRepository : IBaseRepository<PermissionAction>
    {
    }
}
