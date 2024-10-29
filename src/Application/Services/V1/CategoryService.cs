using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.V1;
using Template.Application.Contracts.Services.V1;
using Template.Domain.Entities.V1;

namespace Template.Application.Services.V1
{
    public class CategoryService : ServiceBaseAsync<Category>, ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(
            ICategoryRepository repository,
            IMapper mapper,
            IHttpContextAccessor httpContext,
            IMemoryCache cache,
            ILogger logger,
            IUnitOfWork unitOfWork
            ) : base(repository, mapper, httpContext, cache, logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public override async Task<Category> CreateAsync<TSource>(TSource source)
        {
           using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var categoryRepository = _unitOfWork.Repository<Category>();
                    var itemRepository = _unitOfWork.Repository<Item>();

                    var category = _mapper.Map<Category>(source);
                    await categoryRepository.CreateAsync(category);

                    var item = new Item
                    {
                        Name = $"Item {category.Id}",
                        CategoryId = category.Id,
                        Description = "Item Created by unit of work"
                    };

                    await itemRepository.CreateAsync(item);

                    await _unitOfWork.CommitTransactionAsync();
                    category.Items = [];
                    return category;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
        }
    }
}