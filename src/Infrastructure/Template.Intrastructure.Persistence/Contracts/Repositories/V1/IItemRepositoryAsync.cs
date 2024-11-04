using Template.Domain.Entities.V1;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Contracts.Repositories.V1
{
    public interface IItemRepositoryAsync : IAsyncRepository<Item, ApplicationDbContext>
    {
    }
}