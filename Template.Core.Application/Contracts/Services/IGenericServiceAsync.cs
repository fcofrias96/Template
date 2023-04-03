using Template.Core.Domain.Entities;

namespace Template.Core.Application.Contracts.Services
{
    public interface IGenericServiceAsync<TEntity>
    {
        Task<TDestination> CreateAsync<TSource, TDestination>(TSource source);

        Task DeleteAsync<TKey>(TKey id);

        Task<bool> ExistsAsync<TKey>(TKey id);

        Task<List<TDestination>> GetAllAsync<TDestination>();
        Task<PagedResult<TDestination>> GetAllWithPaginationAsync<TDestination, TSpecification>(IRequest request) where TSpecification : ISpecification<TEntity>;

        Task<(List<TSource>, string)> GetAllWithETagAsync<TSource>();

        Task<TDestination> GetByIdAsync<TKey, TDestination>(TKey id);

        Task UpdateAsync<TKey, TSource>(TKey id, TSource source);
    }
}