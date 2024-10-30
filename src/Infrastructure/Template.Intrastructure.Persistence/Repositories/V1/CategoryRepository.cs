using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Repositories.V1;
using Template.Application.Repository;
using Template.Domain.Entities.V1;

namespace Template.Persistence.Repositories.V1
{
    public class CategoryRepository : RepositoryBaseAsync<Category>, ICategoryRepository
    {
        public CategoryRepository(
            ICreateRepositoryAsync<Category> createRepository,
            IUpdateRepositoryAsync<Category> updateRepository,
            IDeleteRepositoryAsync<Category> deleteRepository,
            IReadRepositoryAsync<Category> readRepository

            ) : base(createRepository, updateRepository,deleteRepository,readRepository)
        {
        }
    }
}