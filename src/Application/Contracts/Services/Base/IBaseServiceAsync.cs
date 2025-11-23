using Microsoft.EntityFrameworkCore;
using Template.Domain.Contracts;

namespace Template.Application.Contracts.Services.Base
{
    public interface IBaseServiceAsync<TEntity> :
        ICreateServiceAsync<TEntity>,
        IDeleteServiceAsync,
        IReadServiceAsync<TEntity>,
        IUpdateServiceAsync
        where TEntity : class, IEntityBase

    {
    }
}