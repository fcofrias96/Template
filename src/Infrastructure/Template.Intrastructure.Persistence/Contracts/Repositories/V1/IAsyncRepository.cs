using Template.Application.Contracts.Repositories.Base;

namespace Template.Persistence.Contracts.Repositories.V1
{
    public interface IAsyncRepository<TEntity, TDbContext> :
        ICreateRepositoryAsync<TEntity, TDbContext>,
        IUpdateRepositoryAsync<TEntity, TDbContext>,
        IDeleteRepositoryAsync<TEntity, TDbContext>,
        IReadRepositoryAsync<TEntity, TDbContext>
    {
    }
}