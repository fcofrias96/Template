using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Repositories.V1;
using Template.Application.Repository;
using Template.Domain.Entities.V1;

namespace Template.Persistence.Repositories.V1
{
    public class ItemRepositoryAsync : RepositoryBaseAsync<Item>, IItemRepositoryAsync
    {
        public ItemRepositoryAsync(
            ICreateRepositoryAsync<Item> createRepository,
            IUpdateRepositoryAsync<Item> updateRepository,
            IDeleteRepositoryAsync<Item> deleteRepository,
            IReadRepositoryAsync<Item> readRepository

            ) : base(createRepository, updateRepository, deleteRepository, readRepository)
        {
        }
    }
}