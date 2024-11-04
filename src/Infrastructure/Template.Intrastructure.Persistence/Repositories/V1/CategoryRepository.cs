using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Domain.Entities.V1;
using Template.Persistence.Contracts.Repositories.V1;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Repositories.V1
{
    public class CategoryRepository : RepositoryBaseAsync<Category, ApplicationDbContext>, ICategoryRepository
    {
        public CategoryRepository(
            ICreateRepositoryAsync<Category, ApplicationDbContext> createRepository,
            IUpdateRepositoryAsync<Category, ApplicationDbContext> updateRepository,
            IDeleteRepositoryAsync<Category, ApplicationDbContext> deleteRepository,
            IReadRepositoryAsync<Category, ApplicationDbContext> readRepository

            ) : base(createRepository, updateRepository,deleteRepository,readRepository)
        {
        }
    }
}