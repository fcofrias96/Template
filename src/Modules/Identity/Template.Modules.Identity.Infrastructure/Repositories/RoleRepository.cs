using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Infrastructure;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Infrastructure.Repositories
{
    public class RoleRepository : BaseRepository<ApplicationRole>, IRoleRepository
    {
        public RoleRepository(IdentityModuleDbContext securityDb) : base(securityDb)
        {
        }
    }
}
