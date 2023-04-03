using System.Linq.Expressions;
using Template.Core.Application.Request;
using Template.Core.Domain.Entities;

namespace Template.Core.Application.Specification
{
    public class ItemSpecification : Specification<Item>
    {
        public ItemSpecification(ItemRequest request) : base(p =>
       (string.IsNullOrEmpty(request.Name) || p.Name.Contains(request.Name)) &&
       (!request.PriceMin.HasValue || p.Price >= request.PriceMin.Value) &&
       (!request.PriceMax.HasValue || p.Price <= request.PriceMax.Value) &&
       (string.IsNullOrEmpty(request.Category) || p.Category.Name.Equals(request.Category, StringComparison.OrdinalIgnoreCase)))
        {
            switch (request?.OrderBy?.ToLower())
            {
                case "name":
                    ApplyOrderBy(p => p.Name);
                    break;
                case "price":
                    ApplyOrderBy(p => p.Price);
                    break;
                case "category":
                    ApplyOrderBy(p => p.Category);
                    break;
                default:
                    ApplyOrderBy(p => p.Id);
                    break;
            }

            if (request.Descending)
            {
                var orderByExpression = OrderBy;
                ApplyOrderByDescending(orderByExpression);
                OrderBy = default;
            }

            ApplyPaging(request.Offset, request.Limit);
        }
    }
}
