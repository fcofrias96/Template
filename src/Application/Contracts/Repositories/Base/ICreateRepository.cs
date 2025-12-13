namespace Template.Application.Contracts.Repositories.Base
{
    public interface ICreateRepository<in TEntity>
    {
        Task CreateAsync(TEntity entity, CancellationToken cancellationToken);
    }
}