using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Domain.Entities.V1;
using Template.Persistence.Contracts.Repositories.V1;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Repositories.V1
{
    public class ItemRepositoryAsync : RepositoryBaseAsync<Item, ApplicationDbContext>, IItemRepositoryAsync
    {
        public ItemRepositoryAsync(
            ICreateRepositoryAsync<Item, ApplicationDbContext> createRepository,
            IUpdateRepositoryAsync<Item, ApplicationDbContext> updateRepository,
            IDeleteRepositoryAsync<Item, ApplicationDbContext> deleteRepository,
            IReadRepositoryAsync<Item, ApplicationDbContext> readRepository

            ) : base(createRepository, updateRepository, deleteRepository, readRepository)
        {
        }
    }
}