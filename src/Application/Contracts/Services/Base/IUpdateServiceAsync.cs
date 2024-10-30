namespace Template.Application.Contracts.Services.Base
{
    public interface IUpdateServiceAsync<TEntity>
    {
        Task UpdateAsync<TKey, TSource>(TKey id, TSource source);
    }
}