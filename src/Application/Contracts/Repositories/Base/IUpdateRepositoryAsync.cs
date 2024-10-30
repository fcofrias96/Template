namespace Template.Application.Contracts.Repositories.Base
{
    public interface IUpdateRepositoryAsync<TEntity>
    {
        Task UpdateAsync(TEntity entity);
    }
}