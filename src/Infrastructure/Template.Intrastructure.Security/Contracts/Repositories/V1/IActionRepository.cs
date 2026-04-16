using Template.Application.Contracts.Repositories.Base;
using Template.Security.DbContexts.V1;
using PermissionAction = Template.Domain.Entities.V1.PermissionAction;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IActionRepository : IBaseRepository<PermissionAction>
    {
    }
}