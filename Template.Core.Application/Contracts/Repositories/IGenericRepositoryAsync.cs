using System.Linq.Expressions;
using Template.Core.Domain.Entities;

namespace Template.Core.Application.Contracts.Repositories
{
    public interface IGenericRepositoryAsync<TEntity>
    {
        Task<TEntity> CreateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task<List<TDestiantion>> GetAllAsync<TDestiantion>(ISpecification<TEntity> spec = default);
   
        Task<TDestination?> GetByIdAsync<TKey, TDestination>(TKey id);

        Task<int> GetTotalCountAsync();

        Task UpdateAsync(TEntity entity);

    }
}