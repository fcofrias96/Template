using Template.Application.Contracts;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Models.Wrappers;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public class ServiceBaseAsync<TEntity,TDbContext> : IAsyncServiceAsync<TEntity, TDbContext> 
        where TEntity : class, IEntityBase
    {
        private readonly ICreateServiceAsync<TEntity, TDbContext> _createService;
        private readonly IDeleteServiceAsync<TEntity, TDbContext> _deleteService;
        private readonly IExportServiceAsync<TEntity, TDbContext> _exportService;
        private readonly IReadServiceAsync<TEntity, TDbContext> _readService;
        private readonly IUpdateServiceAsync<TEntity, TDbContext> _updateService;

        public ServiceBaseAsync(
            ICreateServiceAsync<TEntity, TDbContext> createService,
            IDeleteServiceAsync<TEntity, TDbContext> deleteService,
            IExportServiceAsync<TEntity, TDbContext> exportService,
            IReadServiceAsync<TEntity, TDbContext> readService,
            IUpdateServiceAsync<TEntity, TDbContext> updateService)
        {
            _createService = createService;
            _deleteService = deleteService;
            _exportService = exportService;
            _readService = readService;
            _updateService = updateService;
        }

        public virtual Task<TEntity> CreateAsync<TSource>(TSource source, CancellationToken cancellationToken)
        {
            return _createService.CreateAsync(source, cancellationToken);
        }

        public virtual Task SoftDeleteAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            return _deleteService.SoftDeleteAsync(id, cancellationToken);
        }

        public virtual Task PermanentDeleteAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            return _deleteService.PermanentDeleteAsync(id, cancellationToken);
        }

        public virtual Task<byte[]> ExportToCSVAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters, CancellationToken cancellationToken)
            where TSpecification : ISpecification<TEntity>
        {
            return _exportService.ExportToCSVAsync<TDestination, TSpecification>(title, queryParameters, cancellationToken  );
        }

        public virtual Task<byte[]> ExportToCSVAsync<TDestination>(string title, CancellationToken cancellationToken)
        {
            return _exportService.ExportToCSVAsync<TDestination>(title, cancellationToken);
        }

        public virtual Task<byte[]> ExportToPdfAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters, CancellationToken cancellationToken)
            where TSpecification : ISpecification<TEntity>
        {
            return _exportService.ExportToPdfAsync<TDestination, TSpecification>(title, queryParameters, cancellationToken);
        }

        public virtual Task<byte[]> ExportToPdfAsync<TDestination>(string title, CancellationToken cancellationToken)
        {
            return _exportService.ExportToPdfAsync<TDestination>(title, cancellationToken);
        }

        public virtual Task<byte[]> ExportToDocxAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters, CancellationToken cancellationToken)
            where TSpecification : ISpecification<TEntity>
        {
            return _exportService.ExportToDocxAsync<TDestination, TSpecification>(title, queryParameters, cancellationToken);
        }

        public virtual Task<byte[]> ExportToDocxAsync<TDestination>(string title, CancellationToken cancellationToken)
        {
            return _exportService.ExportToDocxAsync<TDestination>(title, cancellationToken);
        }

        public virtual Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id, CancellationToken cancellationToken)
        {
            return _readService.GetByIdProjectedAsync<TKey, TDestination>(id, cancellationToken);
        }

        public virtual Task UpdateAsync<TKey, TSource>(TKey id, TSource source, CancellationToken cancellationToken )
        {
            return _updateService.UpdateAsync(id, source, cancellationToken);
        }

        public virtual Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>(CancellationToken cancellationToken)
        {
            return _readService.GetProjectedAsync<TDestination>(cancellationToken);
        }

        public virtual Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination, TSpecification>(IQueryParametersBase queryParameters, CancellationToken cancellationToken) where TSpecification : ISpecification<TEntity>
        {
            return _readService.GetProjectedPaginatedAsync<TDestination, TSpecification>(queryParameters,cancellationToken);
        }

        public Task<TEntity> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            return _readService.GetByIdAsync(id, cancellationToken);
        }
    }
}