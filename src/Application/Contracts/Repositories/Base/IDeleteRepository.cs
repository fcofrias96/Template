namespace Template.Application.Contracts.Repositories.Base
{
    public interface IDeleteRepository<in TEntity>
    {
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    }
}