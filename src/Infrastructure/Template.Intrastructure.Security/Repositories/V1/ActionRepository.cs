using AutoMapper;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using PermissionAction = Template.Security.Entities.PermissionAction;

namespace Template.Security.Repositories.V1
{
    public class ActionRepository : BaseRepository<PermissionAction>, IActionRepository
    {
        public ActionRepository(SecurityDbContext securityDb, IConfigurationProvider configurationProvider) : base(securityDb, configurationProvider)
        {
        }
    }
}