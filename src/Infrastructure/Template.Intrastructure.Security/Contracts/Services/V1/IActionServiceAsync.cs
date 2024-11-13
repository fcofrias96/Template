using Template.Application.Models.DTOs.V1.Actions;

namespace Template.Security.Contracts.Services.V1
{
    public interface IActionServiceAsync
    {
        Task<IReadOnlyList<ActionListDto>> GetProjectedAsync();
    }
}