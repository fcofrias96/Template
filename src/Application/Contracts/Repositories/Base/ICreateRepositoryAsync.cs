namespace Template.Application.Contracts.Repositories.Base
{
    public interface ICreateRepositoryAsync<TEntity>
    {
        Task CreateAsync(TEntity entity);
    }
}