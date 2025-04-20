using Serilog;
using Template.Application.Models.Specification;
using Template.Security.Dtos.Roles;
using Template.Security.Entities;

namespace Template.Security.Specification.V1
{
    public class RoleSpecification : Specification<Role>
    {
        public RoleSpecification(RoleQueryParameters queryParameters, bool applyPagination, ILogger logger) : base(queryParameters, applyPagination, logger)
        {
        }
    }
}