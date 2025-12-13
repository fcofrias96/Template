using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Domain.Entities.V1;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Contracts.Services
{
    public interface ICategoryService : IBaseService<Category>, IExportService<Category>
    {
    }
}