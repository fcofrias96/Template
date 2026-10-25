using Template.BuildingBlocks.Application.Contracts.Services.Base;
using Template.BuildingBlocks.Application.Contracts.Services.Base.Export;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Contracts.Services
{
    public interface IItemService : IBaseService<Item>, IExportService<Item>
    {
    }
}
