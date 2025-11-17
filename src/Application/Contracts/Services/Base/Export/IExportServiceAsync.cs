namespace Template.Application.Contracts.Services.Base.Export
{
    public interface IExportServiceAsync<TEntity>
    {
        Task<byte[]> ExportToCSVAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters, CancellationToken cancellationToken) where TSpecification : ISpecification<TEntity>;

        Task<byte[]> ExportToCSVAsync<TDestination>(string title, CancellationToken cancellationToken);

        Task<byte[]> ExportToDocxAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters, CancellationToken cancellationToken) where TSpecification : ISpecification<TEntity>;

        Task<byte[]> ExportToDocxAsync<TDestination>(string title, CancellationToken cancellationToken);

        Task<byte[]> ExportToPdfAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters, CancellationToken cancellationToken) where TSpecification : ISpecification<TEntity>;

        Task<byte[]> ExportToPdfAsync<TDestination>(string title, CancellationToken cancellationToken);
    }
}