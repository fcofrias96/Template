using Template.Application.Models.Wrappers;

namespace Template.Application.Contracts.Services.Base
{
    public interface IReadServiceAsync<TEntity>
    {
        Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>(CancellationToken cancellationToken);

        Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination>(ISpecification<TEntity> specification, CancellationToken cancellationToken);

        Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id, CancellationToken cancellationToken);

        Task<TEntity> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken);
    }
}