using Template.Application.Models.DTOs;

namespace Template.Security.Contracts.Services.V1
{
    public interface IActionServiceAsync
    {
        Task<IReadOnlyList<Catalog>> GetProjectedAsync(CancellationToken cancellationToken);
    }
}