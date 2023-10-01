using System.Linq.Expressions;
using Template.Core.Application.Extensions;
using Template.Core.Application.Request;
using Template.Core.Domain.Entities;

namespace Template.Core.Application.Specification
{
    public class ItemSpecification : Specification<Item>
    {
        public ItemSpecification(ItemRequest request)
        {
            #region Criteria


            if (request.Name != null)
            {
                Expression<Func<Item, bool>> expression = item => false;
                var names = request.Name.Split(',');

                for (int i = 0; i < names.Length; i++)
                {
                    string currentName = names[i];
                    if (i == 0)
                    {
                        expression = item => item.Name.Contains(currentName);
                    }
                    else
                    {
                        expression = expression.Or(item => item.Name.Contains(currentName));
                    }
                }

                Criteria = Criteria is null ? expression : Criteria.And(expression);
            }


            if (request.Categories != null)
            {
                var categories = request.Categories.Split(",").Select(int.Parse).ToArray();
                Expression<Func<Item, bool>> expression = item => categories.Contains(item.CategoryId);

                Criteria = Criteria is null ? expression : Criteria.And(expression);
            }

            #endregion Criteria

            #region OrderBy

            switch (request?.OrderBy.ToLower())
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

            Descending = request?.Descending ?? false;

            #endregion OrderBy

            #region Pagination
            ApplyPaging(request.Offset, request.Limit);
            #endregion Pagination

        }
    }
}
