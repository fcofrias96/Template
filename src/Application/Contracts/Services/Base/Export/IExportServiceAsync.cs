namespace Template.Application.Contracts.Services.Base.Export
{
    public interface IExportServiceAsync<TEntity>
    {
        Task<byte[]> ExportToCSVAsync<TDestination>(string title, ISpecification<TEntity> specification, CancellationToken cancellationToken);

        Task<byte[]> ExportToCSVAsync<TDestination>(string title, CancellationToken cancellationToken);

        Task<byte[]> ExportToDocxAsync<TDestination>(string title, ISpecification<TEntity> specification, CancellationToken cancellationToken);

        Task<byte[]> ExportToDocxAsync<TDestination>(string title, CancellationToken cancellationToken);

        Task<byte[]> ExportToPdfAsync<TDestination>(string title, ISpecification<TEntity> specification, CancellationToken cancellationToken);

        Task<byte[]> ExportToPdfAsync<TDestination>(string title, CancellationToken cancellationToken);
    }
}