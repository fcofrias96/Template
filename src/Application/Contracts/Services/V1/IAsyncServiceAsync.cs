using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;

namespace Template.Application.Contracts.Services.V1
{
    public interface IAsyncServiceAsync<TEntity> :
        ICreateServiceAsync<TEntity>,
        IDeleteServiceAsync<TEntity>,
        IExportServiceAsync<TEntity>,
        IReadServiceAsync<TEntity>,
        IUpdateServiceAsync<TEntity>
    {
    }
}