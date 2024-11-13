using Serilog;
using Template.Application.Contracts;
using Template.Application.Models.Specification;
using Template.Security.Entities;

namespace Template.Security.Specification.V1
{
    public class UserSpecification : Specification<User>
    {
        public UserSpecification(IQueryParametersBase queryParametersBase, bool applyPagination, ILogger logger) : base(queryParametersBase, applyPagination, logger)
        {
        }
    }
}