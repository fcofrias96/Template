using System.Linq.Expressions;

namespace Template.Core.Application.Contracts.Repositories
{
    public interface IGenericRepositoryAsync<TEntity>
    {
        Task<TEntity> CreateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task<List<TDestiantion>> GetAllAsync<TDestiantion>(ISpecification<TEntity> spec = default);
   
        Task<TDestination?> GetByIdAsync<TKey, TDestination>(TKey id);

        Task<int> GetTotalCountAsync(Expression<Func<TEntity, bool>> criteria);

        Task UpdateAsync(TEntity entity);

    }
}