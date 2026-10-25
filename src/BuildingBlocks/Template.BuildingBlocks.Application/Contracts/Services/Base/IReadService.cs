using Template.BuildingBlocks.Application.Caching.Models;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.BuildingBlocks.Application.Models.Wrappers;

namespace Template.BuildingBlocks.Application.Contracts.Services.Base
{
    public interface IReadService<TEntity>
    {
        Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TDestination>(int id, CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>;

        Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>(CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>;

        Task<EtagResult<IReadOnlyList<TDestination>>> GetProjectedWithEtagAsync<TDestination>(CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>;

        Task<EtagResult<IReadOnlyList<TDestination>>> GetProjectedWithEtagAsync<TDestination>(
            ISpecification<TEntity> specification,
            string scopeName,
            int scopeId,
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>;

        Task<EtagResult<TDestination>> GetByIdProjectedWithEtagAsync<TDestination>(int id, CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>;

        Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination>(
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>;

        Task<EtagResult<PagedCollection<TDestination>>> GetProjectedPaginatedWithEtagAsync<TDestination>(
            ISpecification<TEntity> specification,
            string scopeName,
            int? scopeId = default,
            string? sort = default,
            string? filterSignature = default,
            CancellationToken cancellationToken = default)
            where TDestination : IProjection<TEntity, TDestination>;
    }
}
