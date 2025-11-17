namespace Template.Application.Contracts.Repositories.Base
{
    public interface ICreateRepositoryAsync<in TEntity>
    {
        Task CreateAsync(TEntity entity, CancellationToken cancellationToken);
    }
}