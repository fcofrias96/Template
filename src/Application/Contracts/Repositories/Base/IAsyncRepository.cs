namespace Template.Application.Contracts.Repositories.Base
{
    public interface IAsyncRepository<TEntity, TDbContext> :
        ICreateRepositoryAsync<TEntity, TDbContext>,
        IUpdateRepositoryAsync<TEntity, TDbContext>,
        IDeleteRepositoryAsync<TEntity, TDbContext>,
        IReadRepositoryAsync<TEntity, TDbContext>
    {
    }
}