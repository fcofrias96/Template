using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.V1;
using Template.Application.Repository;
using Template.Domain.Contracts;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext context, IConfigurationProvider configurationProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        }

        public IAsyncRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntityBase
        {
            var type = typeof(TEntity);
            if (!_repositories.TryGetValue(type, out var repository))
            {
                repository = new RepositoryBaseAsync<TEntity>(_context, _configurationProvider);
                _repositories[type] = repository;
            }
            return (IAsyncRepository<TEntity>)repository;
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("There is already a transaction in progress.");
            }
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("There are no transactions to confirm.");
            }

            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("There is no transaction to reverse.");
            }

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }


}
