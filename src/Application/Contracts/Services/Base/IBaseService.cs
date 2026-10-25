using Microsoft.EntityFrameworkCore;
using Template.Domain.Contracts;

namespace Template.Application.Contracts.Services.Base
{
    public interface IBaseService<TEntity> :
        ICreateService<TEntity>,
        IDeleteService,
        IReadService<TEntity>,
        IUpdateService<TEntity>
        where TEntity : class, IEntityBase

    {
    }
}