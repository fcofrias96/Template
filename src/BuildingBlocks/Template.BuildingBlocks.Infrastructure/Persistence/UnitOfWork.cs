using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Template.BuildingBlocks.Application.Contracts;

namespace Template.BuildingBlocks.Infrastructure.Persistence
{
    /// <summary>
    /// Transaction boundary over one module's DbContext. Each module exposes its own contract
    /// (e.g. ICatalogUnitOfWork) implemented by a subclass, so modules never share a unit of work.
    /// </summary>
    public abstract class UnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {
        private readonly TContext _context;

        protected UnitOfWork(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> operation,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(operation);

            if (_context.Database.CurrentTransaction is not null)
            {
                await operation(cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    await operation(cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await RollbackSafelyAsync(transaction, cancellationToken);
                    throw;
                }
            });
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(
            Func<CancellationToken, Task<TResult>> operation,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(operation);

            if (_context.Database.CurrentTransaction is not null)
            {
                TResult result = await operation(cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return result;
            }

            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    TResult result = await operation(cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    return result;
                }
                catch
                {
                    await RollbackSafelyAsync(transaction, cancellationToken);
                    throw;
                }
            });
        }

        private static async Task RollbackSafelyAsync(
            IDbContextTransaction transaction,
            CancellationToken cancellationToken)
        {
            try
            {
                await transaction.RollbackAsync(cancellationToken);
            }
            catch
            {
                // Preserve the original exception.
            }
        }
    }
}
