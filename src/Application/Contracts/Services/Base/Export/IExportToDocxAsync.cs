namespace Template.Application.Contracts.Services.Base.Export
{
    public interface IExportToDocxAsync<TEntity, TDbContext>
    {
        Task<byte[]> ExportToDocxAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters, CancellationToken cancellationToken) where TSpecification : ISpecification<TEntity>;

        Task<byte[]> ExportToDocxAsync<TDestination>(string title, CancellationToken cancellationToken);
    }
}