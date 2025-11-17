namespace Template.Application.Contracts.Repositories.Base
{
    public interface IUpdateRepositoryAsync<in TEntity>
    {
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    }
}