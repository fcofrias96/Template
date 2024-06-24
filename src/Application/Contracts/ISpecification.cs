using System.Linq.Expressions;

namespace Template.Application.Contracts
{
    public interface ISpecification<TEntity>
    {
        Expression<Func<TEntity, bool>>? Criteria { get; }
        Expression<Func<TEntity, object>> SortBy { get; }
        int Skip { get; }
        int Take { get; }
        bool Descending { get; }
    }
}