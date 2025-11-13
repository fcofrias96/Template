using Serilog;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Template.Application.Contracts;

namespace Template.Application.Utilities
{
    public static class SpecificationHelper
    {
        // Cache: por cada tipo de Specification guardamos un delegado de creación
        private static readonly ConcurrentDictionary<Type, Delegate> _factoryCache = new();

        public static TSpecification CreatorInstance<TSpecification, TEntity>(
            IQueryParametersBase request,
            ILogger logger,
            bool applyPagination = true
        )
            where TSpecification : ISpecification<TEntity>
        {
            var specType = typeof(TSpecification);

            // Intentamos obtener el factory ya compilado

            var factoryDelegate =  _factoryCache.GetOrAdd(specType, value => CreateFactory<TSpecification>(value));

            var factory = (Func<IQueryParametersBase, bool, ILogger, TSpecification>)factoryDelegate;

            return factory(request, applyPagination, logger);
        }

        private static Delegate CreateFactory<TSpecification>(Type specType)
        {
            // Buscar constructor compatible (no solo exacto)
            var ctor = specType.GetConstructors()
                .FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    return parameters.Length == 3
                        && typeof(IQueryParametersBase).IsAssignableFrom(parameters[0].ParameterType)
                        && parameters[1].ParameterType == typeof(bool)
                        && typeof(ILogger).IsAssignableFrom(parameters[2].ParameterType);
                });

            if (ctor == null)
            {
                throw new InvalidOperationException(
                    $"Specification '{specType.Name}' must have a constructor " +
                    $"with signature (IQueryParametersBase or derived, bool, ILogger or derived).");
            }

            // Params del lambda
            var requestParam = Expression.Parameter(typeof(IQueryParametersBase), "request");
            var applyPaginationParam = Expression.Parameter(typeof(bool), "applyPagination");
            var loggerParam = Expression.Parameter(typeof(ILogger), "logger");

            // Cast explícito al tipo concreto que espera el constructor
            var requestConverted = Expression.Convert(requestParam, ctor.GetParameters()[0].ParameterType);
            var loggerConverted = Expression.Convert(loggerParam, ctor.GetParameters()[2].ParameterType);

            // new TSpecification((TQueryParams)request, applyPagination, (TLogger)logger)
            var newExpr = Expression.New(ctor, requestConverted, applyPaginationParam, loggerConverted);

            // Func<IQueryParametersBase, bool, ILogger, TSpecification>
            var lambda = Expression.Lambda<Func<IQueryParametersBase, bool, ILogger, TSpecification>>(
                newExpr,
                requestParam,
                applyPaginationParam,
                loggerParam
            );

            return lambda.Compile();
        }
    }
}
