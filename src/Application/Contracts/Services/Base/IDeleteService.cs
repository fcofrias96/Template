namespace Template.Application.Contracts.Services.Base
{
    public interface IDeleteService
    {
        Task SoftDeleteAsync(int id, CancellationToken cancellationToken);

        Task PermanentDeleteAsync(int id, CancellationToken cancellationToken);
    }
}  