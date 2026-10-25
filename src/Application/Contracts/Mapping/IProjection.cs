using System.Linq.Expressions;

namespace Template.Application.Contracts.Mapping
{
    public interface IProjection<TEntity, TDestination>
    {
        static abstract Expression<Func<TEntity, TDestination>> Projection { get; }
    }
}

