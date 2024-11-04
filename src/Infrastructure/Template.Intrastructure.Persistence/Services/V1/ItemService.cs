using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services.Base;
using Template.Domain.Entities.V1;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Services.V1
{
    public class ItemService : ServiceBaseAsync<Item, ApplicationDbContext>, IItemService
    {
        public ItemService(
            ICreateServiceAsync<Item, ApplicationDbContext> createService,
            IDeleteServiceAsync<Item, ApplicationDbContext> deleteService,
            IExportServiceAsync<Item, ApplicationDbContext> exportService,
            IReadServiceAsync<Item, ApplicationDbContext> readService,
            IUpdateServiceAsync<Item, ApplicationDbContext> updateService) :
            base(createService, deleteService, exportService, readService, updateService)
        {
        }
    }
}