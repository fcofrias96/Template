namespace Template.Application.Contracts
{
    public interface IUnitOfWork
    {
        Task<bool> ExecuteInTransactionAsync(Func<Task> action);
    }
}