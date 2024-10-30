using AutoMapper;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Identity.Contracts.Repositories.V1;
using Action = Template.Identity.Entities.Action;

namespace Template.Identity.Repositories.V1
{
    public class ActionRepositoryAsync : RepositoryBaseAsync<Action>, IActionRepositoryAsync
    {
        public ActionRepositoryAsync(
            ICreateRepositoryAsync<Action> createRepository,
            IUpdateRepositoryAsync<Action> updateRepository,
            IDeleteRepositoryAsync<Action> deleteRepository,
            IReadRepositoryAsync<Action> readRepository

            ) : base(createRepository, updateRepository, deleteRepository, readRepository)
        {
        }
    }
}