using Template.Application.Models.DTOs.V1.Items;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.Specification.V1
{
    public class ItemSpecification : Specification<Item>
    {
        public ItemSpecification(ItemQueryParameters queryParameters, bool applyPagination) : base(queryParameters, applyPagination)
        {
        }
    }
}