using Template.Application.Contracts.Services.Base;
using Template.Domain.Entities.V1;
using Template.Persistence.DbContexts.V1;

namespace Template.Application.Contracts.Services.V1
{
    public interface ICategoryService : IAsyncServiceAsync<Category, ApplicationDbContext>
    {
    }
}