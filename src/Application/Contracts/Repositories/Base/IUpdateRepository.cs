namespace Template.Application.Contracts.Repositories.Base
{
    public interface IUpdateRepository<TEntity>
    {
        Task UpdateAsync(TEntity entity);
    }

}