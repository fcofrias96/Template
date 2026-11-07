using ServiceStack.Model;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.SharedKernel.Domain;
using Template.SharedKernel.Results;

namespace Template.BuildingBlocks.Application.Contracts.Services.Base
{
    public interface IUpdateService<TEntity> where TEntity : class, IEntityBase
    {
        Task<Result> UpdateAsync<TSource>(int id, TSource source, CancellationToken cancellationToken)
                   where TSource : class, IHasId, IUpdateMapTo<TEntity>;

        Task<Result> PatchAsync<TSource>(int id, TSource source, CancellationToken cancellationToken)
            where TSource : class, IHasId, IUpdateMapTo<TEntity>;
    }
}
