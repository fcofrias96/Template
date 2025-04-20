using Serilog;
using Template.Application.Models.Specification;
using Template.Security.Dtos.Resources;
using Template.Security.Entities;

namespace Template.Security.Specification.V1
{
    public class ResourceSpecification : Specification<Resource>
    {
        public ResourceSpecification(PermissionQueryParameters queryParameters, bool applyPagination, ILogger logger) : base(queryParameters, applyPagination, logger)
        {
        }
    }
}