namespace Template.Application.Contracts.Repositories.Base
{
    public interface IDeleteRepositoryAsync<TEntity>
    {
        Task DeleteAsync(TEntity entity);
    }
}