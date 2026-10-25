using Template.SharedKernel.Domain;

namespace Template.BuildingBlocks.Application.Contracts.Services.Base
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
