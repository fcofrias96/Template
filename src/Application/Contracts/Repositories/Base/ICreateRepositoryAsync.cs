namespace Template.Application.Contracts.Repositories.Base
{
    public interface ICreateRepositoryAsync<TEntity, TDbContext>
    {
        Task CreateAsync(TEntity entity, CancellationToken cancellationToken);
    }
}