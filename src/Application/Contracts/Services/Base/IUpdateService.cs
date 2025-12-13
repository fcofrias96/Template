namespace Template.Application.Contracts.Services.Base
{
    public interface IUpdateService
    {
        Task UpdateAsync<TKey, TSource>(TKey id, TSource source, CancellationToken cancellationToken);
    }
}