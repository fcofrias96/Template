using Serilog;
using Template.Application.Models.DTOs.V1.Categories;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.Specification.V1
{
    public class CategorySpecification : Specification<Category>
    {
        public CategorySpecification(CategoryQueryParameters queryParameters, bool applyPagination, ILogger logger) : base(queryParameters, applyPagination, logger)
        {

        }
    }

}
