using Template.BuildingBlocks.Application.Contracts.Mapping;

namespace Template.BuildingBlocks.Application.Contracts.Services.Base
{
    public interface ICreateService<TEntity>
    {
        Task<TEntity> CreateAsync<TSource>(TSource source, CancellationToken cancellationToken)
            where TSource : ICreateMapTo<TEntity>;
    }
}
