using System.Linq.Expressions;

namespace Template.Application.Contracts.Repositories.Base
{
    public interface IReadRepository<TEntity>
    {
        Task<TEntity?> GetByIdAsync<TKey>(TKey id);
        Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>();
        Task<IReadOnlyList<TDestination>> GetProjectedPaginatedAsync<TDestination>(ISpecification<TEntity> spec);
        Task<int> GetTotalCountAsync(Expression<Func<TEntity, bool>>? criteria);
        Task<TDestination?> GetByIdProjectedAsync<TKey, TDestination>(TKey id);
    }

}