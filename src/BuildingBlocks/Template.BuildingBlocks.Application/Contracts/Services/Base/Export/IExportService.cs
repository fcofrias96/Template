using Template.BuildingBlocks.Application.Contracts.Mapping;

namespace Template.BuildingBlocks.Application.Contracts.Services.Base.Export
{
    public interface IExportService<TEntity>
    {
        Task<byte[]> ExportToCSVAsync<TDestination>(string title, ISpecification<TEntity> specification, CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>;

        Task<byte[]> ExportToCSVAsync<TDestination>(string title, CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>;

        Task<byte[]> ExportToDocxAsync<TDestination>(string title, ISpecification<TEntity> specification, CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>;

        Task<byte[]> ExportToDocxAsync<TDestination>(string title, CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>;

        Task<byte[]> ExportToPdfAsync<TDestination>(string title, ISpecification<TEntity> specification, CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>;

        Task<byte[]> ExportToPdfAsync<TDestination>(string title, CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>;
    }
}
