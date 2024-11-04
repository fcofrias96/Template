using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.DbContexts.V1;
using Action = Template.Identity.Entities.Action;

namespace Template.Identity.Repositories.V1
{
    public class ActionRepositoryAsync : RepositoryBaseAsync<Action, IdentityContext>, IActionRepositoryAsync
    {
        public ActionRepositoryAsync(
            ICreateRepositoryAsync<Action, IdentityContext> createRepository,
            IUpdateRepositoryAsync<Action, IdentityContext> updateRepository,
            IDeleteRepositoryAsync<Action, IdentityContext> deleteRepository,
            IReadRepositoryAsync<Action, IdentityContext> readRepository

            ) : base(createRepository, updateRepository, deleteRepository, readRepository)
        {
        }
    }
}