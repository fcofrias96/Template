namespace Template.Application.Contracts.Repositories.Base
{
    public interface IExportRepository<TEntity>
    {
        Task<IReadOnlyList<TDestination>> GetProjectedForExportAsync<TDestination>(ISpecification<TEntity> spec);
    }

}