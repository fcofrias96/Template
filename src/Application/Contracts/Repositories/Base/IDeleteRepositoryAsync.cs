namespace Template.Application.Contracts.Repositories.Base
{
    public interface IDeleteRepositoryAsync<TEntity, TDbContext>
    {
        Task DeleteAsync(TEntity entity);
    }
}