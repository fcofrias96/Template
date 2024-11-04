namespace Template.Application.Contracts.Repositories.Base
{
    public interface IUpdateRepositoryAsync<TEntity, TDbContext>
    {
        Task UpdateAsync(TEntity entity);
    }
}