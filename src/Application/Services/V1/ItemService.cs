using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services.Base;
using Template.Domain.Entities.V1;

namespace Template.Application.Services.V1
{
    public class ItemService : ServiceBaseAsync<Item>, IItemService
    {
        public ItemService(
            ICreateServiceAsync<Item> createService,
            IDeleteServiceAsync<Item> deleteService,
            IExportServiceAsync<Item> exportService,
            IReadServiceAsync<Item> readService,
            IUpdateServiceAsync<Item> updateService) :
            base(createService, deleteService, exportService, readService, updateService)
        {
        }
    }
}