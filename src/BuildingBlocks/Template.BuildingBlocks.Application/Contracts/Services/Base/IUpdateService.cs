using ServiceStack.Model;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.SharedKernel.Domain;

namespace Template.BuildingBlocks.Application.Contracts.Services.Base
{
    public interface IUpdateService<TEntity> where TEntity : class, IEntityBase
    {
        Task UpdateAsync<TSource>(int id, TSource source, CancellationToken cancellationToken)
                   where TSource : class, IHasId, IUpdateMapTo<TEntity>;

        Task PatchAsync<TSource>(int id, TSource source, CancellationToken cancellationToken)
            where TSource : class, IHasId, IUpdateMapTo<TEntity>;
    }
}
