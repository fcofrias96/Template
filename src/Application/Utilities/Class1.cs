using Serilog;
using Template.Application.Contracts;

namespace Template.Application.Utilities
{
    public static class SpecificationHelper
    {
        public static TSpecification CreatorInstance<TSpecification, TEntity>(
            IQueryParametersBase request,
            ILogger logger,
            bool applyPagination = true
            ) where TSpecification : ISpecification<TEntity>
        {
            var type = typeof(TSpecification);
            object[] constructorArguments = { request, applyPagination, logger };
            var spec = (TSpecification)Activator.CreateInstance(type, constructorArguments)!;
            return spec;
        }
    }
}