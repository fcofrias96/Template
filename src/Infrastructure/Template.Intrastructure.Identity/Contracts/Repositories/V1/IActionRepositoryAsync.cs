using Template.Application.Contracts.Repositories.V1;
using Action = Template.Identity.Entities.Action;

namespace Template.Identity.Contracts.Repositories.V1
{
    public interface IActionRepositoryAsync : IAsyncRepository<Action>
    {
    }
}