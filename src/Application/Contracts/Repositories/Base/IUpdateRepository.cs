namespace Template.Application.Contracts.Repositories.Base
{
    public interface IUpdateRepository<in TEntity>
    {
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    }
}