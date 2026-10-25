using Microsoft.EntityFrameworkCore;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Infrastructure.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly IdentityModuleDbContext _context;

        public MenuRepository(IdentityModuleDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItem>> GetMenuItemsAsync(CancellationToken cancellationToken)
        {
            return await _context.MenuItems
                                    .Select(m => new MenuItem
                                    {
                                        Id = m.Id,
                                        Title = m.Title,
                                        Url = m.Url,
                                        Icon = m.Icon,
                                        ParentId = m.ParentId,
                                        PermissionId = m.PermissionId,
                                        Order = m.Order
                                    })
                                    .OrderBy(m => m.ParentId)
                                    .ThenBy(m => m.Order)
                                    .ToListAsync(cancellationToken);
        }
    }
}
