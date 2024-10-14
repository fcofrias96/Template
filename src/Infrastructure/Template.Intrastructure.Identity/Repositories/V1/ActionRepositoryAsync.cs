using AutoMapper;
using Template.Application.Repository;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.DbContext.V1;
using Action = Template.Identity.Entities.Action;

namespace Template.Identity.Repositories.V1
{
    public class ActionRepositoryAsync : RepositoryBaseAsync<Action>, IActionRepositoryAsync
    {
        public ActionRepositoryAsync(IdentityContext context, IConfigurationProvider configurationProvider) : base(context, configurationProvider)
        {
        }
    }
}