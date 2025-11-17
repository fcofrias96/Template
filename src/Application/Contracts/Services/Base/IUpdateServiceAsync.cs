namespace Template.Application.Contracts.Services.Base
{
    public interface IUpdateServiceAsync
    {
        Task UpdateAsync<TKey, TSource>(TKey id, TSource source, CancellationToken cancellationToken);
    }
}