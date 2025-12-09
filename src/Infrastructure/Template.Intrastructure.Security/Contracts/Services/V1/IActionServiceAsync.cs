using Template.Application.Contracts.Services.Base;
using ActionType = Template.Security.Entities.ActionType;

namespace Template.Security.Contracts.Services.V1
{
    public interface IActionServiceAsync : IReadServiceAsync<ActionType>
    {
    }
}