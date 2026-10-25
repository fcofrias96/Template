namespace Template.BuildingBlocks.Application.Contracts.Repositories.Base
{
    public interface IDeleteRepository<in TEntity>
    {
        Task HardDeletesAsync(TEntity entity, CancellationToken cancellationToken);

        Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken);
    }
}
