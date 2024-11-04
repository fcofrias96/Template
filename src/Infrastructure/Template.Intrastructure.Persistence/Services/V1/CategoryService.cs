using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services.Base;
using Template.Domain.Entities.V1;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Services.V1
{
    public class CategoryService : ServiceBaseAsync<Category, ApplicationDbContext>, ICategoryService
    {
        public CategoryService(
           ICreateServiceAsync<Category, ApplicationDbContext> createService,
            IDeleteServiceAsync<Category, ApplicationDbContext> deleteService,
            IExportServiceAsync<Category, ApplicationDbContext> exportService,
            IReadServiceAsync<Category, ApplicationDbContext> readService,
            IUpdateServiceAsync<Category, ApplicationDbContext> updateService
            ) : base(createService, deleteService, exportService, readService, updateService)
        {
        }
    }
}