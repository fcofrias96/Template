using ServiceStack.Model;

namespace Template.Application.Contracts.Services.Base
{
    public interface IUpdateService
    {
        Task PatchAsync<TSource>(
            int id, TSource source, 
            CancellationToken cancellationToken) 
            where TSource : class, IHasId;

        Task UpdateAsync<TSource>(
            int id,
            TSource source,
            CancellationToken cancellationToken)
            where TSource : class, IHasId;
    }
}