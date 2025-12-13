using Template.Domain.Contracts;

namespace Template.Application.Contracts.Repositories.Base
{
    public interface IBaseRepository<TEntity> :
        ICreateRepository<TEntity>,
        IUpdateRepository<TEntity>,
        IDeleteRepository<TEntity>,
        IReadRepository<TEntity>
        where TEntity : class, IEntityBase
    {
    }
}