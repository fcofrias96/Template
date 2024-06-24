using Template.Application.Models.Wrappers;

namespace Template.Application.Contracts.Services.V1
{
    public interface IAsyncService<TEntity>
    {
        Task<TDestination> CreateAsync<TSource, TDestination>(TSource source);

        Task DeleteLogicalAsync<TKey>(TKey id);

        Task DeletePermanentAsync<TKey>(TKey id);
        Task<byte[]> ExportToExcelAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters)
        where TSpecification : ISpecification<TEntity>;

        Task<byte[]> ExportToExcelAsync<TDestination>(string title);

        Task<byte[]> ExportToPdfAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters)
        where TSpecification : ISpecification<TEntity>;

        Task<byte[]> ExportToPdfAsync<TDestination>(string title);

        Task<byte[]> ExportToWordAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters)
        where TSpecification : ISpecification<TEntity>;

        Task<byte[]> ExportToWordAsync<TDestination>(string title);

        Task<ApiResponse<List<TDestination>>> GetAllProjectedAsync<TDestination>();

        Task<ApiResponse<PagedCollection<TDestination>>> GetAllProjectedWithPaginationAsync<TDestination, TSpecification>(IQueryParametersBase queryParameters) where TSpecification : ISpecification<TEntity>;

        Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id);

        Task UpdateAsync<TKey, TSource>(TKey id, TSource source);
    }
}