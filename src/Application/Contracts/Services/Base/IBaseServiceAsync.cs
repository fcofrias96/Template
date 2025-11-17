using Microsoft.EntityFrameworkCore;
using Template.Application.Models.Wrappers;
using Template.Domain.Contracts;

namespace Template.Application.Contracts.Services.Base
{
    public interface IBaseServiceAsync<TEntity, TDbContext>:
        ICreateServiceAsync<TEntity>,
        IDeleteServiceAsync,
        IReadServiceAsync<TEntity>,
        IUpdateServiceAsync
        where TEntity : class, IEntityBase where TDbContext : DbContext

    {
    
    }
}