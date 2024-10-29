namespace Template.Application.Contracts.Repositories.Base
{
    public interface ICreateRepository<TEntity>
    {
        Task<TEntity> CreateAsync(TEntity entity);
    }

}