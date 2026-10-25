using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Infrastructure;
using PermissionAction = Template.Modules.Identity.Domain.Entities.PermissionAction;

namespace Template.Modules.Identity.Infrastructure.Repositories
{
    public class ActionRepository : BaseRepository<PermissionAction>, IActionRepository
    {
        public ActionRepository(IdentityModuleDbContext securityDb) : base(securityDb)
        {
        }
    }
}
