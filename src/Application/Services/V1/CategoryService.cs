using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services.Base;
using Template.Domain.Entities.V1;

namespace Template.Application.Services.V1
{
    public class CategoryService : ServiceBaseAsync<Category>, ICategoryService
    {
        public CategoryService(
           ICreateServiceAsync<Category> createService,
            IDeleteServiceAsync<Category> deleteService,
            IExportServiceAsync<Category> exportService,
            IReadServiceAsync<Category> readService,
            IUpdateServiceAsync<Category> updateService
            ) : base(createService, deleteService, exportService, readService, updateService)
        {
        }
    }
}