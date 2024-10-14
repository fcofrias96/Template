using Template.Application.Contracts.Services.V1;
using Action = Template.Identity.Entities.Action;

namespace Template.Identity.Contracts.Services.V1
{
    public interface IActionServiceAsync: IAsyncServiceAsync<Action>
    {
    }
}