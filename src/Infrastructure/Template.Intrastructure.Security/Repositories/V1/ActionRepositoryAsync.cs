using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using ActionType = Template.Security.Entities.ActionType;

namespace Template.Security.Repositories.V1
{
    public class ActionRepositoryAsync : RepositoryBaseAsync<ActionType, SecurityDbContext>, IActionRepositoryAsync
    {
        public ActionRepositoryAsync(
            ICreateRepositoryAsync<ActionType, SecurityDbContext> createRepository,
            IUpdateRepositoryAsync<ActionType, SecurityDbContext> updateRepository,
            IDeleteRepositoryAsync<ActionType, SecurityDbContext> deleteRepository,
            IReadRepositoryAsync<ActionType, SecurityDbContext> readRepository

            ) : base(createRepository, updateRepository, deleteRepository, readRepository)
        {
        }
    }
}