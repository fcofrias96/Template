using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Contracts.Repositories
{
    public interface IMenuRepository
    {
        Task<List<MenuItem>> GetMenuItemsAsync(CancellationToken cancellationToken);
    }
}
