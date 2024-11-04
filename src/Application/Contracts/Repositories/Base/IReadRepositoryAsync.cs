using System.Linq.Expressions;

namespace Template.Application.Contracts.Repositories.Base
{
    public interface IReadRepositoryAsync<TEntity, TDbContext>
    {
        Task<TEntity?> GetByIdAsync<TKey>(TKey id);

        Task<TDestination?> GetByIdProjectedAsync<TKey, TDestination>(TKey id);

        Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>();

        Task<IReadOnlyList<TDestination>> GetProjectedForExportAsync<TDestination>(ISpecification<TEntity> spec);

        Task<IReadOnlyList<TDestination>> GetProjectedPaginatedAsync<TDestination>(ISpecification<TEntity> spec);

        Task<int> GetTotalCountAsync(Expression<Func<TEntity, bool>>? criteria);
    }
}