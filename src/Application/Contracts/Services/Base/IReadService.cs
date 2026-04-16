using Template.Application.Caching.Models;
using Template.Application.Models.DTOs;
using Template.Application.Models.Wrappers;

namespace Template.Application.Contracts.Services.Base
{
    public interface IReadService<TEntity>
    {
        Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>(CancellationToken cancellationToken);


        Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination>(ISpecification<TEntity> specification, CancellationToken cancellationToken);

        Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TDestination>(int id, CancellationToken cancellationToken);

        Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<EtagResult<TDestination>> GetByIdProjectedWithEtagAsync<TDestination>(int id, CancellationToken cancellationToken);


        Task<EtagResult<PagedCollection<TDestination>>> GetProjectedPaginatedWithEtagAsync<TDestination>(
        ISpecification<TEntity> specification,
        string scopeName,
        int? scopeId = default,
        string? sort = default,
        string? filterSignature = default,
        CancellationToken cancellationToken = default);

        Task<EtagResult<IReadOnlyList<TDestination>>> GetProjectedWithEtagAsync<TDestination>(
            ISpecification<TEntity> specification,
            string scopeName,
            int scopeId,
            CancellationToken cancellationToken);

        Task<EtagResult<IReadOnlyList<TDestination>>> GetProjectedWithEtagAsync<TDestination>(CancellationToken cancellationToken);
    }
}