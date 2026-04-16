using Template.Application.Services.Base;
using Template.Domain.Entities.V1;
using Template.Persistence.Contracts.Services;

namespace Template.Persistence.Services.V1
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        public CategoryService(BaseServiceDeps<Category> baseServiceDeps) : base(baseServiceDeps)
        {
        }
    }
}