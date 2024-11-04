using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Services.Base;

namespace Template.Application.Contracts.Services.Base
{
    public interface IAsyncServiceAsync<TEntity, TDbContext> :
        ICreateServiceAsync<TEntity, TDbContext>,
        IDeleteServiceAsync<TEntity, TDbContext>,
        IExportServiceAsync<TEntity, TDbContext>,
        IReadServiceAsync<TEntity, TDbContext>,
        IUpdateServiceAsync<TEntity, TDbContext>
    {
    }
}