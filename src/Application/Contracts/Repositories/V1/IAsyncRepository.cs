using Template.Application.Contracts.Repositories.Base;

namespace Template.Application.Contracts.Repositories.V1
{
    public interface IAsyncRepository<TEntity> :
        ICreateRepositoryAsync<TEntity>,
        IUpdateRepositoryAsync<TEntity>,
        IDeleteRepositoryAsync<TEntity>,
        IReadRepositoryAsync<TEntity>
    {
    }
}