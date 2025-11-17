namespace Template.Application.Contracts
{
    public interface IUnitOfWork<TDbContext>
    {
        Task ExecuteInTransactionAsync( Func<CancellationToken, Task> operation,CancellationToken cancellationToken);
    }
}