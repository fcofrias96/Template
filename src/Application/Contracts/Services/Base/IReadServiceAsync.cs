using Template.Application.Models.Wrappers;

namespace Template.Application.Contracts.Services.Base
{
    public interface IReadServiceAsync<TEntity, TDbContext>
    {
        Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>();

        Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination, TSpecification>(IQueryParametersBase queryParameters) where TSpecification : ISpecification<TEntity>;

        Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id);

        Task<TEntity> GetByIdAsync<TKey>(TKey id);
    }
}