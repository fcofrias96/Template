using System.Linq.Expressions;
using Template.Core.Domain.Entities;

namespace Template.Core.Application.Contracts.Repositories
{
    public interface IGenericRepositoryAsync<TEntity>
    {
        Task<TEntity> CreateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task<List<TDestiantion>> GetAllAsync<TDestiantion>(List<string>? navigationPropertyToInclude = null);
        Task<List<TDestiantion>> GetAllWithPaginationAsync<TDestiantion>(QueryParameters<TDestiantion> queryParameters, List<string>?  navigationPropertyToInclude);

        Task<TEntity> GetByIdAsync<TKey>(TKey id, List<string>?  navigationPropertyToInclude);

        Task<int> GetTotalCountAsync();

        Task UpdateAsync(TEntity entity);

        
    }
}