
using Template.Application.Models.Specification;
using Template.Security.Dtos.Permissions;
using Template.Security.Entities;

namespace Template.Security.Specification.V1
{
    public class PermissionSpecification : Specification<Permission>
    {
        public PermissionSpecification(PermissionQueryParameters queryParameters, bool applyPagination) : base(queryParameters, applyPagination)
        {
        }
    }
}