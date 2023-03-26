using Template.Core.Domain.Entities;

namespace Template.Core.Application.Contracts.Services
{
    public interface IGenericServiceAsync<TEntity>
    {
        Task<TDestination> CreateAsync<TSource, TDestination>(TSource source);

        Task DeleteAsync<TKey>(TKey id);

        Task<bool> ExistsAsync<TKey>(TKey id);
        Task<List<TSource>> GetAllAsync<TSource>(List<string>?  navigationPropertyToInclude = default);
        Task<(List<TSource>, string)> GetAllWithETagAsync<TSource>(List<string>?  navigationPropertyToInclude = default);
        Task<PagedResult<TDestination>> GetAllWithPaginationAsync<TDestination>(QueryParameters<TDestination> queryParameters, List<string>?  navigationPropertyToInclude = default);

        Task<TDestination> GetByIdAsync<TKey, TDestination>(TKey id, List<string>?  navigationPropertyToInclude = default);
        Task<(TDestination, string)> GetByIdWithETagAsync<TKey, TDestination>(TKey id, List<string>?  navigationPropertyToInclude = default);

        Task UpdateAsync<TKey, TSource>(TKey id, TSource source);
    }
}