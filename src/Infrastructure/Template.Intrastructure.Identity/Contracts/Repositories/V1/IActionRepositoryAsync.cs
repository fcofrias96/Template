using Template.Application.Contracts.Repositories.Base;
using Template.Identity.DbContexts.V1;
using Action = Template.Identity.Entities.Action;

namespace Template.Identity.Contracts.Repositories.V1
{
    public interface IActionRepositoryAsync : IAsyncRepository<Action, IdentityContext>
    {
    }
}