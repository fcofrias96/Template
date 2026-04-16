namespace Template.Application.Contracts.Repositories.Base
{
    public interface IReadRepository<TEntity>
    {
        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<TDestination?> GetByIdProjectedAsync<TDestination>(int id, CancellationToken cancellationToken);

        Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>(CancellationToken cancellationToken);
        Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>(ISpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<IReadOnlyList<TDestination>> GetProjectedForExportAsync<TDestination>(ISpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<IReadOnlyList<TDestination>> GetProjectedPaginatedAsync<TDestination>(ISpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<int> GetTotalCountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<(DateTimeOffset LastModifiedAt, int Count)> GetListMetadataAsync(ISpecification<TEntity>? spec = default, CancellationToken cancellationToken = default);
        Task<byte[]?> GetRowVersionAsync(int id, CancellationToken cancellationToken);
    }
}