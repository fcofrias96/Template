using System.Linq.Expressions;
using Template.Application.Extensions;
using Template.Application.Models.DTOs.V1.Item;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.Specification.V1
{
    public class ItemSpecification : Specification<Item>
    {
        public ItemSpecification(ItemQueryParameters queryParameters, bool applyPagination) : base(queryParameters, applyPagination)
        {
            #region Criteria

            if (queryParameters.Name != null)
            {
                Expression<Func<Item, bool>> expression = item => item.Name.Contains(queryParameters.Name);

                Criteria = Criteria is null ? expression : Criteria.And(expression);
            }

            if (queryParameters.Category != null)
            {
                Expression<Func<Item, bool>> expression = item => item.Category.Name.Contains(queryParameters.Category);

                Criteria = Criteria is null ? expression : Criteria.And(expression);
            }

            if (queryParameters.MinPrice != null)
            {
                Expression<Func<Item, bool>> expression = item => item.Price >= queryParameters.MinPrice;

                Criteria = Criteria is null ? expression : Criteria.And(expression);
            }

            if (queryParameters.MaxPrice != null)
            {
                Expression<Func<Item, bool>> expression = item => item.Price <= queryParameters.MaxPrice;

                Criteria = Criteria is null ? expression : Criteria.And(expression);
            }

            #endregion Criteria
        }
    }
}