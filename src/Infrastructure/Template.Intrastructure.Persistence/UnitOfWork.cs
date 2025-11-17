using Microsoft.EntityFrameworkCore;
using Template.Application.Contracts;

namespace Template.Persistence
{
    public class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext>
        where TDbContext : DbContext
    {
        private readonly TDbContext _context;

        public UnitOfWork(TDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken)
        {
            // Opcional: si ya hay una transacción, puedes reusar o anidar.
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await operation(cancellationToken);

                // Guardar cambios de todas las entidades trackeadas
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}