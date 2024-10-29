using Template.Application.Contracts.Repositories.Base;

namespace Template.Application.Contracts.Repositories.V1
{
    public interface IAsyncRepository<TEntity> :
        ICreateRepository<TEntity>,
        IUpdateRepository<TEntity>,
        IDeleteRepository<TEntity>,
        IReadRepository<TEntity>,
        IExportRepository<TEntity>
    {

    }

}