namespace Template.Application.Contracts.Services.Base
{
    public interface IDeleteService
    {
        Task SoftDeleteAsync<TKey>(TKey id, CancellationToken cancellationToken);

        Task PermanentDeleteAsync<TKey>(TKey id, CancellationToken cancellationToken);
    }
}