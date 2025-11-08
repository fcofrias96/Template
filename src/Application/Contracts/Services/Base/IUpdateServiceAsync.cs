namespace Template.Application.Contracts.Services.Base
{
    public interface IUpdateServiceAsync<TEntity, TDbContext>
    {
        Task UpdateAsync<TKey, TSource>(TKey id, TSource source, CancellationToken cancellationToken);
    }
}