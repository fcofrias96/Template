using AutoMapper;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using ActionType = Template.Security.Entities.ActionType;

namespace Template.Security.Repositories.V1
{
    public class ActionRepositoryAsync : BaseRepositoryAsync<ActionType, SecurityDbContext>, IActionRepositoryAsync
    {
        public ActionRepositoryAsync(SecurityDbContext securityDb, IConfigurationProvider configurationProvider) : base(securityDb, configurationProvider)
        {
        }
    }
}