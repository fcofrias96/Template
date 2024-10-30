namespace Template.Application.Contracts.Services.Base
{
    public interface IDeleteServiceAsync<TEntity>
    {
        Task SoftDeleteAsync<TKey>(TKey id);

        Task PermanentDeleteAsync<TKey>(TKey id);
    }
}