using Template.Application.Contracts;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Contracts.Services.V1;
using Template.Application.Models.Wrappers;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public class ServiceBaseAsync<TEntity> : IAsyncServiceAsync<TEntity> where TEntity : class, IEntityBase
    {
        private readonly ICreateServiceAsync<TEntity> _createService;
        private readonly IDeleteServiceAsync<TEntity> _deleteService;
        private readonly IExportServiceAsync<TEntity> _exportService;
        private readonly IReadServiceAsync<TEntity> _readService;
        private readonly IUpdateServiceAsync<TEntity> _updateService;

        public ServiceBaseAsync(
            ICreateServiceAsync<TEntity> createService,
            IDeleteServiceAsync<TEntity> deleteService,
            IExportServiceAsync<TEntity> exportService,
            IReadServiceAsync<TEntity> readService,
            IUpdateServiceAsync<TEntity> updateService)
        {
            _createService = createService;
            _deleteService = deleteService;
            _exportService = exportService;
            _readService = readService;
            _updateService = updateService;
        }

        public virtual Task<TEntity> CreateAsync<TSource>(TSource source)
        {
            return _createService.CreateAsync(source);
        }

        public virtual Task SoftDeleteAsync<TKey>(TKey id)
        {
            return _deleteService.SoftDeleteAsync(id);
        }

        public virtual Task PermanentDeleteAsync<TKey>(TKey id)
        {
            return _deleteService.PermanentDeleteAsync(id);
        }

        public virtual Task<byte[]> ExportToExcelAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters)
            where TSpecification : ISpecification<TEntity>
        {
            return _exportService.ExportToExcelAsync<TDestination, TSpecification>(title, queryParameters);
        }

        public virtual Task<byte[]> ExportToExcelAsync<TDestination>(string title)
        {
            return _exportService.ExportToExcelAsync<TDestination>(title);
        }

        public virtual Task<byte[]> ExportToPdfAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters)
            where TSpecification : ISpecification<TEntity>
        {
            return _exportService.ExportToPdfAsync<TDestination, TSpecification>(title, queryParameters);
        }

        public virtual Task<byte[]> ExportToPdfAsync<TDestination>(string title)
        {
            return _exportService.ExportToPdfAsync<TDestination>(title);
        }

        public virtual Task<byte[]> ExportToWordAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters)
            where TSpecification : ISpecification<TEntity>
        {
            return _exportService.ExportToWordAsync<TDestination, TSpecification>(title, queryParameters);
        }

        public virtual Task<byte[]> ExportToWordAsync<TDestination>(string title)
        {
            return _exportService.ExportToWordAsync<TDestination>(title);
        }

        public virtual Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id)
        {
            return _readService.GetByIdProjectedAsync<TKey, TDestination>(id);
        }

        public virtual Task UpdateAsync<TKey, TSource>(TKey id, TSource source)
        {
            return _updateService.UpdateAsync(id, source);
        }

        public virtual Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>()
        {
            return _readService.GetProjectedAsync<TDestination>();
        }

        public virtual Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination, TSpecification>(IQueryParametersBase queryParameters) where TSpecification : ISpecification<TEntity>
        {
            return _readService.GetProjectedPaginatedAsync<TDestination, TSpecification>(queryParameters);
        }

        public virtual Task<TEntity> GetByIdAsync<TKey>(TKey id)
        {
            return _readService.GetByIdAsync(id);
        }
    }
}