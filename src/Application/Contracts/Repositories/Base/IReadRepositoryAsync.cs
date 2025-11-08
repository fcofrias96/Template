using System.Linq.Expressions;

namespace Template.Application.Contracts.Repositories.Base
{
    public interface IReadRepositoryAsync<TEntity, TDbContext>
    {
        Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken);

        Task<TDestination?> GetByIdProjectedAsync<TKey, TDestination>(TKey id, CancellationToken cancellationToken);

        Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>(CancellationToken cancellationToken);

        Task<IReadOnlyList<TDestination>> GetProjectedForExportAsync<TDestination>(ISpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<IReadOnlyList<TDestination>> GetProjectedPaginatedAsync<TDestination>(ISpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<int> GetTotalCountAsync(Expression<Func<TEntity, bool>>? criteria, CancellationToken cancellationToken);
    }
}