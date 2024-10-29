namespace Template.Application.Contracts.Repositories.Base
{
    public interface IDeleteRepository<TEntity>
    {
        Task DeleteAsync(TEntity entity);
    }

}