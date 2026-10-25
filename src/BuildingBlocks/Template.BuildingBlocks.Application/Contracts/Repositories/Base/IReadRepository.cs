using System.Linq.Expressions;

namespace Template.BuildingBlocks.Application.Contracts.Repositories.Base
{
    public interface IReadRepository<TEntity>
    {
        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<TDestination?> GetByIdProjectedAsync<TDestination>(
            int id,
            Expression<Func<TEntity, TDestination>> projection,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>(
            Expression<Func<TEntity, TDestination>> projection,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>(
            ISpecification<TEntity> spec,
            Expression<Func<TEntity, TDestination>> projection,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<TDestination>> GetProjectedPaginatedAsync<TDestination>(
            ISpecification<TEntity> spec,
            Expression<Func<TEntity, TDestination>> projection,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<TDestination>> GetProjectedForExportAsync<TDestination>(
            ISpecification<TEntity> spec,
            Expression<Func<TEntity, TDestination>> projection,
            CancellationToken cancellationToken);

        Task<int> GetTotalCountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<(DateTimeOffset LastModifiedAt, int Count)> GetListMetadataAsync(
            ISpecification<TEntity>? spec = default,
            CancellationToken cancellationToken = default);

        Task<byte[]?> GetRowVersionAsync(int id, CancellationToken cancellationToken);
    }
}
