using Template.Application.Contracts.Repositories.Base;
using Template.Security.DbContexts.V1;
using PermissionAction = Template.Security.Entities.PermissionAction;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IActionRepository : IBaseRepository<PermissionAction>
    {
    }
}