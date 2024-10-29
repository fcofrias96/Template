using Template.Application.Contracts.Repositories.V1;
using Template.Domain.Contracts;

namespace Template.Application.Contracts
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IAsyncRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntityBase;
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

}