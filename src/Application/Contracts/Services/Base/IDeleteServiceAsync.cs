namespace Template.Application.Contracts.Services.Base
{
    public interface IDeleteServiceAsync<TEntity, TDbContext>
    {
        Task SoftDeleteAsync<TKey>(TKey id);

        Task PermanentDeleteAsync<TKey>(TKey id);
    }
}