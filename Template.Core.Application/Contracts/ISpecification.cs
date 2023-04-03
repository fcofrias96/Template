using AutoMapper;
using System.Linq.Expressions;

namespace Template.Core.Application.Contracts
{
    public interface ISpecification<TEntity>
    {
        Expression<Func<TEntity, bool>> Criteria { get; }
        Expression<Func<TEntity, object>> OrderBy { get; }
        Expression<Func<TEntity, object>> OrderByDescending { get; }
        int Skip { get; }
        int Take { get; }
        bool IsPagingEnabled { get; }

    }
}
