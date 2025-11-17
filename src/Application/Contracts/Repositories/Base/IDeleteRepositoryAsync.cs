namespace Template.Application.Contracts.Repositories.Base
{
    public interface IDeleteRepositoryAsync<in TEntity>
    {
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    }
}