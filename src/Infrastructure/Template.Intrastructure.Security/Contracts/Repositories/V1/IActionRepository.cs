using Template.Application.Contracts.Repositories.Base;
using Template.Security.DbContexts.V1;
using ActionType = Template.Security.Entities.ActionType;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IActionRepository : IBaseRepository<ActionType>
    {
    }
}