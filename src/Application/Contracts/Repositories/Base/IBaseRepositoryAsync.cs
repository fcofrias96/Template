using Template.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Template.Application.Contracts.Repositories.Base
{
    public interface IBaseRepositoryAsync <TEntity, TDbContext>:
        ICreateRepositoryAsync<TEntity>,
        IUpdateRepositoryAsync<TEntity>, 
        IDeleteRepositoryAsync<TEntity>, 
        IReadRepositoryAsync<TEntity>
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {

    }
}
